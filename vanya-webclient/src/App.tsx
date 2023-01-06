import type { Component } from 'solid-js';
import {Routes, Route, Router, Link} from '@solidjs/router'

import logo from './logo.svg';
import styles from './App.module.css';

import {InstrumentList} from './instrument-list'
import {Market} from './market'

const App: Component = () => {
  return (
  <Router>
    <div>
      <Link href='/'>Project Vanya</Link>
    </div>
    <Routes>
      <Route path="/market/:id" component={Market}></Route>
      <Route path="/" component={InstrumentList}></Route>
    </Routes>
    </Router>
  );
};

export default App;