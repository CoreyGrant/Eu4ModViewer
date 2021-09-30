<template>
	<div class="app-container">
		<nav class="mod-navigation">
			<div class="nav-start">
				<router-link to="/Index">Home</router-link>
			</div>
			<ul>
				<li>
					<router-link :to="'/Mod/' + id + '/countries'">
						Countries
						<app-icon base="countries" style="height: 24px; width: 24px;"/>
					</router-link>
				</li>
				<li>
					<router-link :to="'/Mod/' + id + '/ideaGroups'">
						Idea Groups
						<app-icon base="ideas" style="height: 24px; width: 24px;"/>
					</router-link>
				</li>
				<li>
					<router-link :to="'/Mod/' + id + '/policies'">
						Policies
						<app-icon base="policies" style="height: 24px; width: 24px;"/>
					</router-link>
				</li>
				<li>
					<router-link :to="'/Mod/' + id + '/religionGroups'">
						Religions
						<app-icon base="religions" style="height: 24px; width: 24px;"/>
					</router-link>
				</li>
			</ul>
		</nav>
		<div class="page">
			<a :href="steamLink(id)" target="_blank" style="font-size: 2rem">{{name}}</a>
			<router-view></router-view>
		</div>
	</div>
</template>
<script lang="ts">
	import Vue from 'vue';
	import AppIcon from './Components/appIcon.vue';
	import { getModDetails } from './appData';

	export default Vue.extend({
		components: {
			AppIcon
		},
		data(): any {
			return {
				name: ''
			};
		},
		mounted() {
			getModDetails(this.id).then(x => this.name = x.name);
		},
		props: {
			id: String,
		},
		methods: {
			steamLink(modId): string {
				return +modId
					? "https://steamcommunity.com/sharedfiles/filedetails/?id=" + modId
					: "https://store.steampowered.com/app/236850/Europa_Universalis_IV/";
			}
		}
	});
</script>