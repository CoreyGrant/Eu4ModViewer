<template>
    <div class="app-container">
		<div class="queue-mod">
			<h2>Add a mod</h2>
			<div class="row">
				<div class="col-12 col-sm-6 d-flex flex-column">
					<label>Id <input class="eu4-input" v-model="id" /></label>
					<p v-if="errorMessage">{{errorMessage}}</p>
					<button class="eu4-btn-sm" @click="validateMod()" :disabled="!id && !id.length" v-if="!(idValid && idValidated)">Validate</button>
				</div>
				<div class="col-12 col-sm-6 d-flex flex-column">
					<p>Exclude files:</p>
					<div class="d-flex align-items-center">
						<label style="margin-bottom: 0;">Filename (without ext) <input v-model="excludeFilename" class="eu4-input" /></label><button class="eu4-btn-add" @click="addExclude()" :disabled="!excludeFilename"></button>
					</div>
					<div class="d-flex flex-row flex-wrap">
						<div class="eu4-tag" v-for="exc in excludeFiles" @click="removeExclude(exc)"><p>{{exc}}</p></div>
					</div>
				</div>
				<div class="col-12">
					<div v-if="idValid && (!details.title || !details.title.length) && (!details.title || !details.title.length)">
						<p>The mod doesn't have any details for the name and description. Please enter them here.</p>
						<label>Name <input class="form-control eu4-input" v-model="overrideTitle" /></label>
						<label>Description <textarea class="form-control eu4-input" v-model="overrideDesc" /></label>
					</div>
					<div v-if="idValid && details.title && details.description">
						<h4>{{details.title}}</h4>
					</div>
				</div>
				<div class="col-12 d-flex justify-content-center align-items-center">
					<button class="eu4-btn-sm" @click="submitMod()" v-if="idValid && idValidated" :disabled="!idValid || !idValidated || !overrideValid">Submit</button>
				</div>
			</div>
			
		</div>
		<div class="mod-queue eu4-table">
			<h2>Queued mods</h2>
			<div class="row title-row">
				<div class="col-2">Id</div>
				<div class="col-5">Name</div>
				<div class="col-2">Status</div>
				<div class="col-3"></div>
			</div>
			<expanding-scroll-list>
				<div class="row" v-for="queuedMod in queuedMods">
					<div class="col-2">{{queuedMod.modId}}</div>
					<div class="col-5">{{queuedMod.title}}</div>
					<div class="col-2">{{getModStatus(queuedMod.status)}}</div>
					<div class="col-3"><button class="eu4-btn-sm" @click="rerunMod(queuedMod)" v-if="queuedMod.status === 3">Re-run</button></div>
				</div>
			</expanding-scroll-list>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import { queuedMods, modExists, queueMod, modQueued, getPublishedFileDetails } from './appData';
	import AppIcon from './Components/appIcon.vue';
	import ExpandingScrollList from './Components/expandingScrollList.vue';

	export default Vue.extend({
		components: {
			AppIcon,
			ExpandingScrollList
		},
		data(): any {
			return {
				idValid: false,
				idValidated: false,
				errorMessage: '',
				id: '',
				queuedMods: [],
				details: {},
				overrideTitle: '',
				overrideDesc: '',
				excludeFiles: [],
				excludeFilename: '',
			};
		},
		created() {
			this.loadQueuedMods();
		},
		computed:{
			overrideValid(): boolean {
				if (!this.details.description && (!this.overrideDesc || !this.overrideDesc.length)) {
					return false;
				}
				if (!this.details.title && (!this.overrideTitle || !this.overrideTitle.length)) {
					return false;
				}
				return true;
			}
		},
		methods: {
			addExclude() {
				this.excludeFiles.push(this.excludeFilename);
				this.excludeFilename = '';
			},
			removeExclude(exclude) {
				this.excludeFiles = this.excludeFiles.filter(x => x !== exclude);
			},
			rerunMod(mod) {
				this.id = mod.modId;
				this.validateMod();
			},
			getModStatus(status): string {
				switch (status) {
					case 0:
						return "Pending";
					case 1:
						return "Processing";
					case 2:
						return "Succeeded";
					case 3:
						return "Failed";
				}
				return "";
			},
			async validateMod() {
				var modId = this.id;
				if (!Number.isInteger(+modId)) {
					this.idValid = false;
					this.errorMessage = 'The id must be a number';
				} else {
					if (await modExists(+modId)) {
						this.idValid = false;
						this.errorMessage = 'The mod already exists';
					} else if (await modQueued(+modId)) {
						this.idValid = false;
						this.errorMessage = 'The mod is already queued';
					} else {
						this.idValid = true;
					}
				}
				if (this.idValid) {
					this.details = await getPublishedFileDetails(+modId);
				}
				this.idValidated = true;
			},
			async submitMod() {
				var request = {
					modId: +this.id,
					description: this.details.description || this.overrideDesc,
					title: this.details.title || this.overrideTitle,
					lastUpdated: this.details.lastUpdated || new Date(),
					excludeFiles: this.excludeFiles.join('|')
				};
				await queueMod(request);
				this.id = '';
				this.overrideDesc = '';
				this.overrideTitle = '';
				this.details = {};
				this.excludeFiles = [];
				this.excludeFilename = '';
				await this.loadQueuedMods()
			},
			async loadQueuedMods() {
				queuedMods().then(x => this.queuedMods = x);
			}
		},
		watch: {
			id: function () {
				this.idValid = false;
				this.idValidated = false;
				this.errorMessage = '';
			}
		}

	});
</script>