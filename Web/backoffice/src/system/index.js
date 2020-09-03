import React, { useRef, useEffect } from "react"
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
import { Code as AdminCode, Content as AdminContent } from "./admins"
import { Code as GroupCode, Content as GroupContent } from "./groups"
import {
  Code as DepartmentCode,
  Content as DepartmentContent
} from "./departments"
import { SideNav } from "bumbag"
import PageNotFound from "../page/NotFound"

export const Code = "system"

export const DefaultUrl = `/${Code}`

const DefaultCodePath = [Code]
//TODO: i18n
const name = () => "系统设置"

export const Menu = ({
  title = name(),
  url = DefaultUrl,
  codes,
  codePaths = DefaultCodePath,
  ...props
}) => {
  let authCodes = useAuthCodes(codePaths)
  codes = codes ?? authCodes

  return (
    <SideNav.Level title={title}>
      <SideNav.Level>
        {codes.map((code) => (
          <MenuItem key={code} code={code} url={url} />
        ))}
      </SideNav.Level>
    </SideNav.Level>
  )
}

const MenuItem = ({ code, url, ...props }) => {
  let ref = useRef()
  let path = `${url}/${code}`

  useEffect(() => {
    console.log("TODO: blur?")
    ref?.current.blur()
  })

  return (
    <SideNav.Item navId={path}>
      <Link ref={ref} to={path}>
        {label(code)}
      </Link>
    </SideNav.Item>
  )

  //TODO: seems cause loop on debug mode
  /*
  let ref = useRef()
  let { pathname } = useLocation()
  let path = `${url}/${code}`
  let isActive = path == pathname
  useEffect(() => {
    if (isActive) {
      ref?.current.focus()
    }
  }, [isActive])

  return (
    <SideNav.Item navId={path} isActive={isActive}>
      <Link ref={ref} to={path}>
        {label(code)}
      </Link>
    </SideNav.Item>
  )
  */
}

//TODO: i18n
const label = (code = "") => {
  switch (code) {
    case AdminCode:
      return "管理员列表"
    case GroupCode:
      return "权限组管理"
    case DepartmentCode:
      return "部门管理"
    default:
      return ""
  }
}

export const Content = ({ codes, ...props }) => {
  let route = useRouteMatch()

  let codePaths = [Code]
  let authCodes = useAuthCodes(codePaths)
  codes = codes ?? authCodes

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
    case AdminCode:
      return <AdminContent />
    case GroupCode:
      return <GroupContent />
    case DepartmentCode:
      return <DepartmentContent />
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
