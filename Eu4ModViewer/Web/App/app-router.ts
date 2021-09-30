import Vue from 'vue';
import Router from 'vue-router';
import GenericModComponent from './genericModComponent.vue';
import ModQueue from './modQueue.vue';
import Countries from './Components/Section/countries.vue';
import IdeaGroups from './Components/Section/ideaGroups.vue';
import Policies from './Components/Section/policies.vue';
import GreatProjects from './Components/Section/greatProjects.vue';
import ReligionGroups from './Components/Section/religionGroups.vue';
import Index from './index.vue';
import About from './about.vue';

Vue.use(Router);

var routes = [
    {
        path: "/",
        redirect: "/Index"
    },
    {
        path: "/Mod/:id",
        component: GenericModComponent,
        props: route => ({ id: route.params.id }),
        children: [
            {
                path: 'countries',
                component: Countries,
                props: route => ({ id: route.params.id })
            },
            {
                path: 'ideaGroups',
                component: IdeaGroups,
                props: route => ({ id: route.params.id })
            },
            {
                path: 'religionGroups',
                component: ReligionGroups,
                props: route => ({ id: route.params.id })
            },
            {
                path: 'policies',
                component: Policies,
                props: route => ({ id: route.params.id })
            },
        ],
    },
    {
        path: "/Index",
        component: Index,
    },
    {
        path: "/About",
        component: About,
    },
    {
        path: "/ModQueue",
        component: ModQueue,
    }
];

var router = new Router({
    mode: 'hash',
    routes: routes,
});

export default {
    router,
    routes
};