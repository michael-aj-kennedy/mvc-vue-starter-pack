import { createApp } from 'vue';
import store from "../store";
import App from "../../components/example/test.js";

export function init() {
    createApp(App)
        .use(store) // Properly inject the Vuex store into the application
        .mount('#app-home'); // Mount the app
}