import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import { Switch, Route, Link, useParams, useRouteMatch } from "react-router-dom"
import {
  PageContent,
  PageWithHeader,
  PageWithSidebar,
  Button,
  Stack,
  Card
} from "bumbag"
import Sidebar from "./Sidebar"
import useSWR, { cache } from "swr"
import BrandPage from "./brand/Page"

function App() {
  let { url, path } = useRouteMatch()
  //TODO: remove defaultIsVisible={false}
  return (
    <PageWithSidebar sidebar={<Sidebar />}>
      <PageContent>
        <Switch>
          <Route exact path="/"></Route>
          <Route path={`/:brandCode`}>
            <BrandPage />
          </Route>
        </Switch>
      </PageContent>
    </PageWithSidebar>
  )
}

export default App
