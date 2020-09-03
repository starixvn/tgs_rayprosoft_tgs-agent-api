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
import { PageContent } from "bumbag"

export const Code = "departments"

export const Content = () => {
  return <PageContent>TODO: list departments</PageContent>
}
