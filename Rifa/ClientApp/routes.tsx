import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Edit } from "./components/Edit";

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/edit/:id' component={ Edit } />
</Layout>;
