<template>
	<div class="app-container">
		<div class="index-page">
			<div class="row mt-3">
				<div class="col-md-12">
					<p>The app is still in development, so features may be in flux and there will be bugs. If you want to report a bug, request a feature or mod, or just ask a question, the Discord is <a href="https://discord.gg/HMKN7tY7zV" target="_blank">here</a></p>
				</div>
			</div>
			<div class="row mt-3">
				<div class="col-md-12">
					<p>To add a mod, or see which mods are currently being processed, go to the <router-link to="/ModQueue">mod queue</router-link></p>
				</div>
			</div>
			<div class="mod-list">
				<div class="row">
					<div class="col-12">
						<div class="filters mod-filters">
							<div class="name-input d-flex">
								<label for="name-search" class=" mr-3">Name</label>
								<input id="name-search" type="text" class="form-control eu4-input" v-model="search.name" />
							</div>
							<p class="ml-4">{{filteredMods.length}} mods</p>
						</div>
					</div>
				</div>
				<div class="eu4-table">
					<div class="row title-row">
						<div class="col-3">Mod</div>
						<div class="col-9">Sections</div>
					</div>
					<div class="row" v-for="mod in filteredMods">
						<div class="col-3"><a :href="steamLink(mod)" target="_blank">{{mod.name}}</a></div>
						<div class="col-9 d-flex align-items-center">
							<router-link class="ml-4" :to="'/Mod/' + mod.modId + '/countries'">Countries <app-icon base="countries" style="width: 24px; height: 24px;" /></router-link>
							<router-link class="ml-4" :to="'/Mod/' + mod.modId + '/ideaGroups'">Idea Groups <app-icon base="ideas" style="width: 24px; height: 24px;" /></router-link>
							<router-link class="ml-4" :to="'/Mod/' + mod.modId + '/policies'">Policies <app-icon base="policies" style="width: 24px; height: 24px;" /></router-link>
							<router-link class="ml-4" :to="'/Mod/' + mod.modId + '/religionGroups'">Religions <app-icon base="religions" style="width: 24px; height: 24px;" /></router-link>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import { getMods } from './appData';
	import AppIcon from './Components/appIcon.vue'

	export default Vue.extend({
		components: {
			AppIcon
		},
		props:{
		},
		data() : any{
			return {
				search: {
					name: '',
				},
				mods: []
			};
		},
		created(): void{
			getMods().then(x => this.mods = x);
		},
		computed: {
			filteredMods(): any[] {
				var searchName = this.search.name;
				var mods = this.mods;
				if (searchName) {
					mods = mods.filter(x => x.name.toLowerCase().includes(searchName.toLowerCase()));
				}
				return mods.sort((x,y) => x.name.toLowerCase() > y.name.toLowerCase());
			}
		},
		methods: {
			steamLink(mod): string {
				return mod.modId
					? "https://steamcommunity.com/sharedfiles/filedetails/?id=" + mod.modId
					: "https://store.steampowered.com/app/236850/Europa_Universalis_IV/"
			}
		}
	});
</script>
