<template>
    <div class="religion" v-bind:style="getReligionStyle(religion)">
        <div class="d-flex flex-row justify-content-between">
            <h4>{{religion.name | religionName}}</h4>
        </div>
        <div v-if="religion.country">
            <p class="sideways align-self-center justify-self-center">global</p>
            <div>
                <bonus :name="name" :value="value" v-for="(value, name) in religion.country" :key="name"></bonus>
            </div>
        </div>
        <div v-if="religion.province">
            <p class="sideways align-self-center justify-self-center">local</p>
            <div>
                <bonus :name="name" :value="value" v-for="(value, name) in religion.province" :key="name"></bonus>
            </div>
        </div>
        <div v-if="religion.secondaryCountry">
            <p class="sideways align-self-center justify-self-center">syncr</p>
            <div>
                <bonus :name="name" :value="value" v-for="(value, name) in religion.secondaryCountry" :key="name"></bonus>
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
			religion: Object
        },
        computed: {
            
		},
		methods:{
			getReligionStyle(religion) {
				if (!religion.color || !religion.color.rgb) {
					return {};
				}
				return {
					"border": "3px solid " + religion.color.rgb,
				};
			},
		},
        data() {
            return {

			}
		}
    });
</script>