import React from "react"
import { css } from "@emotion/core"
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link,
  Redirect,
  useHistory,
  useLocation,
  useRouteMatch
} from "react-router-dom"
import { useAuthCodes } from "../user/context"
import {
  Code as PermissionCode,
  Menu as PermissionsMenu,
  Content as PermissionContent
} from "./permissions"
import { SideNav } from "bumbag"
import PageNotFound from "../page/NotFound"

export const Code = "system"

export const DefaultUrl = `/${Code}`

//TODO: i18n
const name = () => "系统设置"

export const Menu = ({ title = name(), url = DefaultUrl, codes, ...props }) => {
  let codePaths = [Code]
  let authCodes = useAuthCodes(codePaths)
  codes = codes ?? authCodes

  //TODO: rm
  codes = [PermissionCode]
  console.log("TODO: rm system menu codes", [PermissionCode])

  return (
    <SideNav.Level title={title}>
      {codes.map((code) => (
        <Item key={code} code={code} codePaths={codePaths} />
      ))}
    </SideNav.Level>
  )
}

export const Content = ({ codes, ...props }) => {
  let route = useRouteMatch()

  let codePaths = [Code]
  let authedCodes = useAuthCodes(codePaths)
  codes = codes ?? authedCodes

  //TODO: not auth content?
  codes = [PermissionCode]
  console.log("TODO: rm system content codes ", [PermissionCode])

  return (
    <Switch>
      {codes.map((code) => {
        let url = path(route?.path, code)

        return (
          <Route key={code} path={url}>
            <Body code={code} />
          </Route>
        )
      })}
      <Route>
        <PageNotFound />
      </Route>
    </Switch>
  )
}

const Body = ({ code }) => {
  switch (code) {
    case PermissionCode:
      return <PermissionContent />
    default:
      return <PageNotFound />
  }
}

const Item = ({ code, codePaths }) => {
  switch (code) {
    case PermissionCode:
      return <PermissionsMenu url={DefaultUrl} codePaths={codePaths} />
  }
}

const path = (url = "/", path) => `${url}/${path}`
