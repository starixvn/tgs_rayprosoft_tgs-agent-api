import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
import { PageContent, PageWithHeader, PageWithSidebar, Button } from "bumbag"
import Sidebar from "./Sidebar"
import { PlayerTable } from "./player/Table"
import { GameTable } from "./game/Table"

function App() {
  let { path, url } = useRouteMatch()
  //TODO: remove defaultIsVisible={false}
  return (
    <PageWithSidebar defaultIsVisible={false} sidebar={<Sidebar />}>
      <PageContent>
        <Switch>
          <Route exact path="/">
            <PlayerTable />
          </Route>
          <Route path={`/:playerCode`}>
            <GameTable />
          </Route>
        </Switch>
      </PageContent>
    </PageWithSidebar>
  )
}

export default App
