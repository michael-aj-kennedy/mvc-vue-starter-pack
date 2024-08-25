import { mapState } from "vuex";
import template from './test.html';
import "./test.scss";

export default {
    data() {
        return {
            message: "Hello World with .NET MVC and Vue.js!!!"
        };
    },
    computed: {
        ...mapState(["anotherMessage"])
    },
    methods: {
        changeMessage() {
            this.$store.commit("changeMessage");
        }
    },
    template: template
};