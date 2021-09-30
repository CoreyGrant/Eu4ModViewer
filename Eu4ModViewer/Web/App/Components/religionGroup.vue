<template>
	<div class="religion-group">
		<div class="row religion-group-top">
			<div class="col-9 d-flex justify-content-start">
				<h3>{{religionGroup.name | religionName}}</h3>
			</div>
			<div class="col-3 d-flex justify-content-end align-items-center">
				<app-icon class="bonus-image" bonus="defender_of_faith" v-if="religionGroup.defenderOfFaith"/>
				<app-icon class="bonus-image" bonus="personal_union" v-if="religionGroup.canFormPersonalUnions"/>
			</div>
		</div>
		<div class="religions">
			<div class="row">
				<div class="col col-xs-12 col-md-6 col-lg-4 col-xl-3" v-for="religion in religionGroup.religions">
					<religion :religion="religion" :key="religion.name"></religion>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import Religion from './religion.vue';
	import AppIcon from './appIcon.vue';

	export default Vue.extend({
		components: {
			Religion,
			AppIcon
		},
		props:{
			religionGroup: Object
		},
		methods:{
		},
		data() {
			return {

			};
		}
    });
</script>