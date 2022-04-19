import * as React from "react";
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/home/Home';
import {Audio} from './components/audio/audio';
import './custom.css'
import {Signup} from "./components/signUp/Signup";
import {Unsubscribe} from "./components/unsubscribe/Unsubscribe";

export const App = () => {
    return (
        <Layout>
            <Route exact path='/' component={Home} />
            <Route exact path='/signup' component={Signup} />
            <Route exact path='/audio' component={Audio} />
            <Route exact path='/unsubscribe/:userId' component={Unsubscribe} />
        </Layout>
    );
};