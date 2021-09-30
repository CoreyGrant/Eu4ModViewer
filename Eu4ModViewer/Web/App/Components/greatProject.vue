<template>
	<div class="great-project">
		<div class="d-flex justify-content-between">
			<div class="great-project-details">
				<h2>{{greatProject.name | bonusName}}</h2>
				<p>Can it be moved: {{greatProject.canBeMoved ? 'Yes': 'No'}}</p>
				<p>Province: {{greatProject.startName}} ({{greatProject.start}})</p>
			</div>
			<div v-if="any(greatProject.canUseModifiers)" class="great-project-trigger-list">
				<div>
					<div>
						<h4>Who can use it:</h4>
					</div>
					<div v-if="greatProject.canUseModifiers.anyReligion && greatProject.canUseModifiers.anyReligion.length" v-for="a in greatProject.canUseModifiers.anyReligion">
						<p>Religion: {{a | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.anyReligionGroup && greatProject.canUseModifiers.anyReligionGroup.length" v-for="a in greatProject.canUseModifiers.anyReligionGroup">
						<p>Religion group: {{a | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.anyCulture && greatProject.canUseModifiers.anyCulture.length" v-for="a in greatProject.canUseModifiers.anyCulture">
						<p>Culture: {{a | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.anyCultureGroup && greatProject.canUseModifiers.anyCultureGroup.length" v-for="a in greatProject.canUseModifiers.anyCultureGroup">
						<p>Culture group: {{a | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.religion && greatProject.canUseModifiers.religion.length">
						<p>Religion: {{greatProject.canUseModifiers.religion | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.religionGroup && greatProject.canUseModifiers.religionGroup.length">
						<p>Religion group: {{greatProject.canUseModifiers.religionGroup | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.culture && greatProject.canUseModifiers.culture.length">
						<p>Culture: {{greatProject.canUseModifiers.culture | bonusName}}</p>
					</div>
					<div v-if="greatProject.canUseModifiers.cultureGroup && greatProject.canUseModifiers.cultureGroup.length">
						<p>Culture group: {{greatProject.canUseModifiers.cultureGroup | bonusName}}</p>
					</div>
				</div>
			</div>
		</div>
		<div class="great-project-row">
			<div style="border-right: none;"></div>
			<div class="d-flex align-items-center justify-content-center" :class="{startingTier: greatProject.startingTier == 1}">
				<p>Tier 1</p>
			</div>
			<div class="d-flex align-items-center justify-content-center" :class="{startingTier: greatProject.startingTier == 2}">
				<p>Tier 2</p>
			</div>
			<div class="d-flex align-items-center justify-content-center" :class="{startingTier: greatProject.startingTier == 3}">
				<p>Tier 3</p>
			</div>
		</div>
		<div class="great-project-row table-border" v-if="(greatProject.tier1.provinceModifiers && greatProject.tier1.provinceModifiers.length) || (greatProject.tier2.provinceModifiers && greatProject.tier2.provinceModifiers.length) || (greatProject.tier3.provinceModifiers && greatProject.tier3.provinceModifiers.length)">
			<div class="d-flex justify-content-center align-items-center"><h4>Local</h4></div>

			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier1.provinceModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier2.provinceModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier3.provinceModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
		</div>
		<div class="great-project-row table-border" v-if="(greatProject.tier1.areaModifiers && greatProject.tier1.areaModifiers.length) || (greatProject.tier2.areaModifiers && greatProject.tier2.areaModifiers.length) || (greatProject.tier3.areaModifiers && greatProject.tier3.areaModifiers.length)">
			<div class="d-flex justify-content-center align-items-center"><h4>Area</h4></div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier1.areaModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier2.areaModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier3.areaModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
		</div>
		<div class="great-project-row table-border" v-if="(greatProject.tier1.countryModifiers && greatProject.tier1.countryModifiers.length) || (greatProject.tier2.countryModifiers && greatProject.tier2.countryModifiers.length) || (greatProject.tier3.countryModifiers && greatProject.tier3.countryModifiers.length)">
			<div class="d-flex justify-content-center align-items-center"><h4>Global</h4></div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier1.countryModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier2.countryModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
			<div class="d-flex flex-column bonus-detail">
				<bonus v-for="(value, name) in greatProject.tier3.countryModifiers" :name="name" :value="value" :key="name"></bonus>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
    import Vue from 'vue';
	import AppIcon from './appIcon.vue';
	import Bonus from './bonus.vue';

    export default Vue.extend({
		components:{
			AppIcon,
			Bonus
		},
		props:{
			greatProject: Object
		},
		methods:{
			any(obj) {
				var keys = Object.keys(obj);
				if (!keys.length) {
					return false
				}
				var values = keys.map(x => obj[x]);
				return values.some(x => x && x.length);
			}
		},
		data() {
			return {

			};
		},
		computed:{
		}
    });
</script>