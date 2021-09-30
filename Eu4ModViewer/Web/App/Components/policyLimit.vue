<template>
	<span :title="title">&lt;Max&gt;</span>
</template>
<script lang = "ts">
	import Vue from 'vue';
	import AppIcon from './appIcon.vue';

	export default Vue.extend({
		components: {
			AppIcon
		},
		props: {
			allow: Object,
			policyNameDisplayNameMap: Object
		},
		data() {
			return {
			};
		},
		mounted() {
		},
		methods: {
		},
		computed: {
			title() {
				var amountCs = this.allow.conditionSets.find(x => x.conditionSets && x.conditionSets[0].conditions.some(y => y.name == "amount")).conditionSets[0];
				var amount = amountCs.conditions.find(x => x.name == "amount").value;
				var policyNames = amountCs.conditions.filter(x => x.name == "has_active_policy").map(x => x.value);
				var lines = [`The policy is only available with less than\n${amount} of the following policies:`];
				policyNames.forEach(x => {
					lines.push(this.policyNameDisplayNameMap[x]);
				});
				return lines.join("\n");
			}
		},
		watch: {
		}
	});
</script>