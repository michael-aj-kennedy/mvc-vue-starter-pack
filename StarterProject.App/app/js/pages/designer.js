import { createApp } from 'vue';
import store from "../store";
import App from "../../components/designer/designer.js";

export function init() {
    createApp(App)
        .use(store) // Properly inject the Vuex store into the application
        .mount('#app-designer'); // Mount the app
};