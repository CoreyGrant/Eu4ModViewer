<template>
    <expanding-scroll-list>
		<div class="great-projects">
			<great-project :key="project.name" :great-project="project" v-for="project in greatProjects">
			</great-project>
		</div>
    </expanding-scroll-list>
</template>

<script lang="ts">
	import Vue from 'vue';
	import GreatProject from '../../Components/greatProject.vue';
	import ExpandingScrollList from '../../Components/expandingScrollList.vue';
	import { getJson } from '../../appData';
	
	export default Vue.extend({
		components: {
			GreatProject,
			ExpandingScrollList
		},
		props:{
			id: Number,
		},
		data(): any{
			return {
				listLength: 20,
				greatProjects: [],
			}
		},
		created(): void {
			getJson("greatProjects", this.id).then(x => this.greatProjects = x.greatProjects);
		},
		methods: {
		},
		computed: {
		}
	});
</script>