import { createStore } from 'vuex'   // npm install vuex

export default createStore({
    state() {
        return {
            userdata: {}
        }
    },
    mutations: {
        authenticate(state, userdata) {
            if (!userdata) {
                state.userdata = {};
                return;
            }
            state.userdata = userdata;
        }
    }
});
