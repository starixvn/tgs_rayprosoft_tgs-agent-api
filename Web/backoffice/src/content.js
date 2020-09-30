import React, { useRef, useEffect } from "react"
import { css } from "@emotion/core"
import {
  Switch as _Switch,
  Route,
  Link,
  useRouteMatch,
  useLocation
} from "react-router-dom"
import { useMenuCodes } from "./user/context"
import { Route as AuthRouter } from "./user/auth"
import PageNotFound from "./page/NotFound"
import memoize from "fast-memoize"
import { codeMenuContentPair as platformPair } from "./platform"
import { codeMenuContentPair as playerManagementPair } from "./player-management"
import { codeMenuContentPair as systemPair } from "./system"
import { PageContent } from "bumbag"

export const Switch = ({ codes, path, pairs = [], ...props }) => {
  let route = useRouteMatch()
  let authCodes = useMenuCodes(path)
  codes = codes ?? authCodes

  return (
    <_Switch>
      {codes.map((code) => {
        return (
          <Route key={code} path={`${route?.path}/${code}`}>
            <ValueByCode code={code} pairs={pairs} />
          </Route>
        )
      })}
      <Route>
        <PageNotFound />
      </Route>
    </_Switch>
  )
}

export const AllContents = ({ codes }) => {
  let authCodes = useMenuCodes()
  codes = codes ?? authCodes
  let list = pairs()
  return (
    <_Switch>
      {codes.map((code) => {
        let path = `/${code}`

        return (
          <AuthRouter key={code} path={path}>
            <ContentByCode code={code} pairs={list} />
          </AuthRouter>
        )
      })}

      <AuthRouter path="*">
        <PageContent></PageContent>
      </AuthRouter>
    </_Switch>
  )
}

const pairs = () => [platformPair, playerManagementPair, systemPair]

const ValueByCode = ({ code = "", pairs = [] }) => {
  let [, Value] = pairs.find(([id]) => id === code) ?? []
  if (Value) {
    return <Value />
  }

  return null
}

const ContentByCode = ({ code = "", pairs = [] }) => {
  let [, pair] = pairs.find(([id]) => id === code) ?? []
  if (pair) {
    let [, Content] = pair
    return <Content />
  }

  return null
}
