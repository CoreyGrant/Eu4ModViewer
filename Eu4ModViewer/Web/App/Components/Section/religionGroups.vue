<template>
	<div class="religions-page">
		<expanding-scroll-list>
			<div class="religion-groups">
				<religion-group v-for="religionGroup in religionGroups" :religion-group="religionGroup" :key="religionGroup.displayName"></religion-group>
			</div>
		</expanding-scroll-list>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import ReligionGroup from '../../Components/religionGroup.vue';
	import ExpandingScrollList from '../../Components/expandingScrollList.vue';
	import { getJson } from '../../appData';

	export default Vue.extend({
		components: {
			ReligionGroup,
			ExpandingScrollList
		},
		props: {
			id: Number,
		},
		data() {
			return {
				religionGroups: [],
				bonuses: []
			};
		},
		created(): void {
			getJson("religionGroups", this.id).then(x => this.religionGroups = x.religionGroups);
			getJson("bonuses", this.id).then(x => this.bonuses = x.bonuses);
		},
		methods: {
		},
		computed: {
			
		}
	});
</script>