import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Edit } from "./components/Edit";
import { Admin } from "./components/Admin";
import { EditAdmin } from "./components/EditAdmin";

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/edit/:id' component={ Edit } />
    <Route path='/admin' component={ Admin } />
    <Route path='/editadmin/:id' component={ EditAdmin } />
</Layout>;
