<template>
	<div class="countries-page">
		<div class="filters">
			<div class="bonus-input">
				<label for="idea-bonus-search">Bonus</label>
				<select v-model="search.bonus" class="form-control eu4-input" id="idea-bonus-search">
					<option :value='null'>None</option>
					<option v-for="bonusKey in bonuses" :value="bonusKey">{{bonusKey | bonusName}}</option>
				</select>
			</div>
			<div class="category-input">
				<label for="country-bonus-search">Name</label>
				<input id="country-bonus-search" type="text" class="form-control eu4-input" v-model="search.name" />
			</div>
			<p class="ml-4">{{filteredCountries.length}} countries</p>
		</div>
		<div class="countries">
			<expanding-scroll-list @load-more="listLength += 20">
				<div class="countries-list">
					<country v-for="country in pagedCounties" :modId="id" :key="country.name" :country="country" :active-bonus="search.bonus"></country>
				</div>
			</expanding-scroll-list>
		</div>
	</div>
</template>

<script lang="ts">
	import Vue from 'vue';
	import Country from '../../Components/country.vue';
	import { getJson } from '../../appData';
	import ExpandingScrollList from '../../Components/expandingScrollList.vue';
	import { getBonusName } from '../../Helpers/getBonusUrl';

	export default Vue.extend({
		components: {
			Country,
			ExpandingScrollList
		},
		props: {
			id: Number,
		},
		data(): any {
			return {
				search: {
					bonus: <String | null>null,
					name: ''
				},
				listLength: 20,
				countries: [],
				bonuses: []
			};
		},
		created(): void {
			getJson("countries", this.id).then(x => this.countries = x.countries);
			getJson("bonuses", this.id).then(x => this.bonuses = x.bonuses.sort((x, y) => getBonusName(x) > getBonusName(y)));
			if (this.$route.query.name) {
				this.search.name = this.$route.query.name;
			}
		},
		methods: {
		},
		computed: {
			filteredCountries: function (): any[] {
				var searchName = this.search.name;
				var searchBonus = this.search.bonus;
				var countries = <any[]>this.countries;
				if (searchBonus !== null) {
					countries = countries.filter(x => x.ideas.some(y => Object.keys(y.bonuses).some(z => z === searchBonus)));
				}
				if (searchName.length) {
					countries = countries.filter(x => x.name && x.name.toLowerCase().includes(searchName.toLowerCase())
					);
				}
				return countries || [];
			},
			pagedCounties(): any[] {
				return this.filteredCountries.slice(0, this.listLength);
			}
		},
		watch: {
			search: {
				handler() {
					this.listLength = 20;
				},
				deep: true
			},
		}
	});
</script>
