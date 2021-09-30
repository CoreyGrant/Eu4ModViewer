<template>
	<div class="idea-groups-page">
		<div class="filters">
			<div class="bonus-input">
				<label for="idea-bonus-search">Bonus</label>
				<select v-model="search.bonus" class="form-control eu4-input" id="idea-bonus-search">
					<option :value='null'>None</option>
					<option v-for="bonus in bonuses" :value="bonus">{{bonus | bonusName}}</option>
				</select>
			</div>
			<div class="monarch-power-input">
				<img :src="baseUrl + 'Icons/ADM.png'" :class="{active: search.monarch === 'ADM'}" @click="toggleMonarch('ADM')" />
				<img :src="baseUrl + 'Icons/DIP.png'" :class="{active: search.monarch === 'DIP'}" @click="toggleMonarch('DIP')" />
				<img :src="baseUrl + 'Icons/MIL.png'" :class="{active: search.monarch === 'MIL'}" @click="toggleMonarch('MIL')" />
			</div>
			<p class="ml-4">{{filteredIdeaGroups.length}} idea groups</p>
		</div>
		<div class="idea-groups">
			<expanding-scroll-list @load-more="listLength += 20">
				<div class="countries-list">
					<idea-group v-for="idea in pagedIdeaGroups" :key="idea.name" :idea-group="idea" :active-bonus="search.bonus">
					</idea-group>
				</div>
			</expanding-scroll-list>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import IdeaGroup from '../../Components/ideaGroup.vue';
	import ExpandingScrollList from '../../Components/expandingScrollList.vue';
	import { getJson } from '../../appData';
	import { getBonusName } from '../../Helpers/getBonusUrl';

	export default Vue.extend({
		components: {
			IdeaGroup,
			ExpandingScrollList
		},
		props: {
			id: Number,
		},
		data(): any {
			return {
				baseUrl: window.location.origin + '/',
				listLength: 20,
				search: {
					bonus: null,
					monarch: null,
				},
				bonuses: [],
				ideaGroups: [],
			}
		},
		created(): void {
			getJson("ideaGroups", this.id).then(x => this.ideaGroups = x.ideaGroups);
			getJson("bonuses", this.id).then(x => this.bonuses = x.bonuses.sort((x, y) => getBonusName(x) > getBonusName(y)));
		},
		methods: {
			toggleMonarch(val): void {
				this.search.monarch = this.search.monarch === val
					? null
					: val;
			},
		},
		computed: {
			filteredIdeaGroups(): any[] {
				var activeBonusFilter = this.search.bonus;
				var activeMonarchFilters = this.search
					.monarch;
				var ideas = this.ideaGroups;
				if (activeBonusFilter) {
					ideas =
						ideas.filter(idea => {
							var bonusTypes = idea.ideas.map(x => Object.keys(x.bonuses)).flat();
							return bonusTypes.indexOf(activeBonusFilter) > -1;
						});
				}
				if (activeMonarchFilters !== null) {
					ideas = ideas.filter(x => x.category == activeMonarchFilters);
				}
				return ideas.sort((a, b) => a.name > b.name) || [];
			},
			pagedIdeaGroups: function (): any[] {
				return this.filteredIdeaGroups.slice(0, this.listLength);
			}
		},
		watch: {
			search: {
				handler() {
					this.listLength = 20;
				},
				deep: true
			},
		}
	});
</script>