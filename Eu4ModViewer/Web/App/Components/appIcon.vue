<template>
	<img :src="fullUrl" :class="className" @click="$emit('click')"/>
</template>

<script lang="ts">
    import Vue from 'vue';
	import { getImageLink, getIconLink } from '../appData';

    export default Vue.extend({
		props:{
			bonus: String,
			ui: String,
			base: String,
			flag: String,
			modId: String,
			className: Object
		},
		data() {
			return {
			};
		},
		methods:{
		},
		computed:{
			fullUrl(): string{
				if (this.flag) {
					return getImageLink(this.flag, "flags", this.modId)
				}
				if (this.bonus) {
					return getIconLink(this.bonus, "Bonuses");
				}
				if (this.ui) {
					return getIconLink(this.ui, "UI");
				}
				if (this.base) {
					return getIconLink(this.base);
				}
				return "";
			}
		}
    });
</script>