<template>
	<div class="country">
		<div class="country-bonuses">
			<div class="d-flex flex-row justify-content-between country-bonuses-title">
				<div class="flag-image" v-if="countryColor" v-bind:style="{'background-color': countryColor}"></div>
				<div style="width: 100%" class="d-flex justify-content-center">
					<h4 style="margin-top: 4px; font-size: 1.6rem;">{{country.name}}</h4>
				</div>
				<app-icon v-if="country.tag" class="flag-image" :modId="modId" :flag="country.tag"></app-icon>
			</div>
			<div class="country-idea-group-bonus d-flex flex-row" v-for="(idea, index) in orderedIdeas">
				<div style="width: 16px;" class="d-flex align-items-center justify-content-center">
					<p v-if="idea.name == 'start'" style="font-size: 12px; writing-mode: vertical-lr; text-orientation: mixed;">trad</p>
					<p v-if="idea.name == 'bonus'" style="font-size: 12px; writing-mode: vertical-lr; text-orientation: mixed;">+</p>
					<p v-if="idea.name != 'bonus' && idea.name != 'start'">{{index}}</p>
				</div>
				<div class="d-flex flex-column ml-1" :title="$options.filters.bonusName(idea.name)">
					<bonus v-for="(value, name) in idea.bonuses" :name="name" :value="value" :activeBonus="activeBonus == name" :key="name"/>
				</div>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
    import Vue from 'vue';
	import AppIcon from './appIcon.vue';
	import Bonus from './bonus.vue';

	export default Vue.extend({
		components: {
			AppIcon,
			Bonus
		},
		props:{
			country: Object,
			activeBonus: String,
			modId: String
		},
		methods:{

		},
		data() {
			return {
			};
		},
		computed: {
			countryColor: function (): string {
				var country = this.country;
				if (!country.colors || !country.colors.length) {
					return "";
				}
				var color = country.colors[0];

				return`rgb(${color.red},${color.green},${color.blue})`
			},
			orderedIdeas: function():any[]{
				var ideas = [...this.country.ideas];
				var final = ideas.find(x => x.name.toLowerCase() == "bonus");
				return [
					... ideas.filter(x => x.name.toLowerCase() !== "bonus"),
					final
				];
			}
		}
    });
</script>