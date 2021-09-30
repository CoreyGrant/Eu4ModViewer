<template>
	<div class="app-header">
		<div class="app-header-icon d-flex justify-content-center align-items-center ml-2 mt-2">
			<app-icon base="app-icon"/>
		</div>
		<div class="green-banner-small-2 app-header-title d-flex justify-content-center align-items-center ml-4 mr-4" style="margin-top: -6px;">
			<h2 class="m0 p0" @click="$router.push('/Index')" style="cursor: pointer; margin-top: -4px;">EU4 Mod Viewer</h2>
		</div>
		<a href="https://discord.gg/HMKN7tY7zV" target="_blank" class="mr-2 mt-2"><img style="width: 100px;" src="Icons/Discord-Logo.png" /></a>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import AppIcon from './appIcon.vue';

	export default Vue.extend({
		components: {
			AppIcon,
		},
		data() {
			return {
			};
		},
		
	});
</script>
