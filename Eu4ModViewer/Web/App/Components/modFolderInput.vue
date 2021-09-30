<template>
	<div class="row">
		<div class="col-12">
			<label>Files <input type="text" :value="config.files.join(',')" v-on:input="listChange(files, $event)"/></label>
			<label>Ignore <input type="text" :value="config.files.join(',')" v-on:input="listChange(ignore, $event)"/></label>
			<div class="row">
				<div class="col-3">
					<label><input type="checkbox" v-model="config.all" /> All</label>
				</div>
				<div class="col-3">
					<label><input type="checkbox" v-model="config.base"/> Base</label>
				</div>
				<div class="col-3">
					<label><input type="checkbox" v-model="config.baseNames"/> Base Names</label>
				</div>
				<div class="col-3">
					<label><input type="checkbox" v-model="config.collapse"/> Collapse</label>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
    import Vue from 'vue';

	export default Vue.extend({
		components: {
		},
		props:{
			value: Object
		},
		methods:{
			filesChange: function(key, event): void{
				this.config.files = event.target.value.split(',');
			},
	ignoreChange: function(event): void{
		this.config.ignore = event.target.value.split(',');
	}
		},
		data(): any {
			return {
	files: 'files',
	ignore: 'ignore',
				config:{
					files: [],
					ignore: [],
					baseNames: false,
					base: false,
					collapse: false,
					all: false
				}
			};
		},
		created(): void{
			if(this.value){this.config = this.value;}
		},
		watch: {
			config: {
				handler(): void{
					var val = <any>{};
					if (this.config.files.length) {
						val.files = this.config.files;
					}
					if (this.config.ignore.length) {
						val.ignore = this.config.ignore;
					}
					if (this.config.base) {
						val.base = true;
					}
					if (this.config.baseNames) {
						val.baseNames = true;
					}
					if (this.config.collapse) {
						val.collapse = true;
					}
					if (this.config.all) {
						val.all = true;
					}
					this.$emit('change', val);
				},
				deep: true
			},
		}
	});
</script>