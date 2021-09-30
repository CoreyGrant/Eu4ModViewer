<template>
	<div class="policy">
		<div class="d-flex flex-row justify-content-between country-bonuses-title" style="border-bottom: 2px double rgb(165, 121, 66);">
			<div style="width: 100%" class="d-flex justify-content-center">
				<h4 style="margin-top: 4px; font-size: 1.3rem;">
					{{name}}
					<span v-if="hasAnyOf"> + </span>
					<policy-one-of v-if="hasAnyOf" :allow="policy.allow" :idea-groups="ideaGroups"></policy-one-of>
					<policy-limit v-if="isAmountLimited" :allow="policy.allow" :policy-name-display-name-map="policyNameDisplayNameMap"></policy-limit>
				</h4>
			</div>
			<app-icon :base="policy.monarchPower" class="policy-monarch-power" />
		</div>
		<div class="row">
			<div class="col">
				<div class="d-flex flex-column">
					<bonus v-for="(value, name) in policy.bonuses" :name="name" :value="value" :activeBonus="activeBonus == name" :key="name" />
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
    import Vue from 'vue';
	import AppIcon from './appIcon.vue';
	import Bonus from './bonus.vue';
	import PolicyOneOf from './policyOneOf.vue';
	import PolicyLimit from './policyLimit.vue';

	export default Vue.extend({
		components: {
			AppIcon,
			Bonus,
			PolicyOneOf,
			PolicyLimit
		},
		props:{
			policy: Object,
			activeBonus: Number,
			policyNameDisplayNameMap: Object,
			ideaGroups: Array,
		},
		data() {
			return {
				baseUrl: window.location.origin + '/',
			};
		},
		methods: {
		},
		computed: {
			isAmountLimited(): boolean {
				var conditionSets = this.policy.allow.conditionSets;
				if (conditionSets && conditionSets.length) {
					if (conditionSets.some(cs => cs.conditions.some(x => x.name == "amount"))
						|| conditionSets.some(cs => cs.conditionSets.some(cs2 => cs2.conditions.some(y => y.name == "amount")))
					) {
						return true;
					}
				}
				return false;
			},
			hasAnyOf(): boolean {
				var conditionSets = this.policy.allow.conditionSets;
				if (conditionSets && conditionSets.length) {
					for (var cs of conditionSets) {
						if (cs.composeOr && cs.conditions && cs.conditions.length) {
							return true;
						}
					}
				}
				return false;
			},
			name(): string {
				return this.policyNameDisplayNameMap[this.policy.name];
			}
		}
    });
</script>