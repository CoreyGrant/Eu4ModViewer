<template>
	<div class="policies-page">
		<div class="filters">
			<div class="bonus-input">
				<label for="idea-bonus-search">Bonus</label>
				<select v-model="search.bonus" class="form-control eu4-input" id="policy-bonus-search">
					<option :value='null'>None</option>
					<option v-for="bonus in bonuses" :value="bonus">{{bonus | bonusName}}</option>
				</select>
			</div>
			<div class="bonus-input">
				<label for="idea-bonus-search">Idea</label>
				<select v-model="search.ideaGroup" class="form-control eu4-input" id="policy-bonus-search">
					<option :value='null'>None</option>
					<option v-for="ideaGroup in ideaGroups" :value="ideaGroup.name">{{ideaGroupName(ideaGroup)}}</option>
				</select>
			</div>
			<div class="monarch-power-input">
				<img :src="baseUrl + 'Icons/ADM.png'" :class="{active: search.monarch === 'ADM'}" @click="toggleMonarch('ADM')" />
				<img :src="baseUrl + 'Icons/DIP.png'" :class="{active: search.monarch === 'DIP'}" @click="toggleMonarch('DIP')" />
				<img :src="baseUrl + 'Icons/MIL.png'" :class="{active: search.monarch === 'MIL'}" @click="toggleMonarch('MIL')" />
			</div>
			<p class="ml-4">{{filteredPolicies.length}} policies</p>
		</div>
		<div class="policies">
			<expanding-scroll-list @load-more="listLength += 40">
				<div class="countries-list">
					<policy v-for="policy in pagedPolicies" :policy="policy" :active-bonus="search.bonus" :key="policy.name" :policy-name-display-name-map="nameDisplayNameMap" :idea-groups="ideaGroups"></policy>
				</div>
			</expanding-scroll-list>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import Policy from '../../Components/policy.vue';
	import helpers from '../../Helpers/helpers';
	import ExpandingScrollList from '../../Components/expandingScrollList.vue';
	import { getJson } from '../../appData';
	import { getBonusName } from '../../Helpers/getBonusUrl';

	export default Vue.extend({
		components: {
			Policy,
			ExpandingScrollList
		},
		data(): any {
			return {
				listLength: 40,
				baseUrl: window.location.origin + '/',
				search: {
					bonus: null,
					monarch: null,
					ideaGroup: null
				},
				bonuses: [],
				policies: [],
				ideaGroups: [],
			};
		},
		props: {
			id: Number,
		},
		created(): void {
			getJson("ideaGroups", this.id).then(x => this.ideaGroups = x.ideaGroups);
			getJson("policies", this.id).then(x => {
				this.policies = x.policies;
			});
			getJson("bonuses", this.id).then(x => this.bonuses = x.bonuses.sort((x, y) => getBonusName(x) > getBonusName(y)));

		},
		methods: {
			ideaGroupName(ideaGroup): string {
				return ideaGroup.localizedName || (this.$options as any).filters["bonusName"](ideaGroup.name);
			},
			createName(policy) {
				var allow = policy.allow;
				var conditions = allow.conditions;
				var nameParts: string[] = [];
				for (var condition of conditions) {
					if (condition.name == "full_idea_group") {
						nameParts.push(this.ideaGroups.find(x => x.name == condition.value).localizedName || this.tidyName(condition.value));
					}
				}
				if (nameParts.length) {
					return nameParts.map(x => x.replace(" Ideas", "")).join(" + ");
				}
				var conditionSets = allow.conditionSets;
				for (var cs of conditionSets) {
					if (cs.conditions.some(x => x.name === 'amount') && cs.conditions.some(x => x.name == 'full_idea_group')) {
						var amount = cs.conditions.find(x => x.name == 'amount').value;
						return amount === "1"
							? "1 idea group"
							: amount + " idea groups";
					}
				}
				
			},
			toggleMonarch(val): void {
				this.search.monarch = this.search.monarch === val
					? null
					: val;
			},
			tidyName(name: string): string {
				var deideaedName = name.split("_ideas")[0];
				var tidiedName = (this.$options as any).filters["bonusName"](deideaedName);
				return tidiedName;
			},
		},
		computed: {
			nameDisplayNameMap(): any {
				return this.policies.reduce((prev, cur) => {
					return Object.assign({}, prev, { [cur.name]: this.createName(cur) });
				}, {});
			},
			filteredPolicies: function (): any[] {
				var activeBonusFilter: string | null = this.search.bonus;
				var activeMonarchFilters: string | null = this.search
					.monarch;
				var activeIdeaGroupFilter: string | null = this.search.ideaGroup;
				var policies: any[] = this.policies;
				if (activeBonusFilter !== null) {
					policies =
						policies.filter(x => Object.keys(x.bonuses).some(x => x == activeBonusFilter));
				}
				if (activeMonarchFilters !== null) {
					policies = policies.filter(x => x.monarchPower == activeMonarchFilters);
				}
				if (activeIdeaGroupFilter !== null) {
					policies = policies.filter(helpers.ideaGroupHasPolicy.bind(null, activeIdeaGroupFilter));
				}
				return policies || [];
			},
			pagedPolicies: function (): any[] {
				return this.filteredPolicies.slice(0, this.listLength);
			}
		},
		watch: {
			search: {
				handler() {
					this.listLength = 40;
				},
				deep: true
			},
		}
	});
</script>