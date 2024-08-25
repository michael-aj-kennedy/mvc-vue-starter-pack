import { createApp } from 'vue';
import { mapState } from "vuex";
import store from "../store";

export function init() {
    const app = createApp({
        data() {
            return {};
        },
        computed: {
            ...mapState(["anotherMessage"])
        }
    });

    // Properly inject the Vuex store into the application
    app.use(store);

    // Mount the app
    app.mount('#app-about');
}