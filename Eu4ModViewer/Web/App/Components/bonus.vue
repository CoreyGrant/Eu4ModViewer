<template>
	<div class="bonus" :class="{active: activeBonus}">
		<app-icon :bonus="name" class="bonus-image-small" />{{name | bonusName}}: <span style="font-weight: bold; margin-left: 4px;">{{value | bonusValue(name)}}</span>
	</div>

</template>
<script lang="ts">
	import Vue from 'vue';
	import AppIcon from './appIcon.vue';

	export default Vue.extend({
		components: {
			AppIcon
		},
		props: {
			name: String,
			value: String,
			activeBonus: Boolean
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
		},
		watch: {
		}
	});
</script>