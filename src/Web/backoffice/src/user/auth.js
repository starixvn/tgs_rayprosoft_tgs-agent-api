import React, { createContext, useContext, useState } from "react"
import {
  BrowserRouter as Router,
  Switch,
  Route as RouteBase,
  Redirect,
  Link,
  useHistory,
  useLocation,
  useRouteMatch
} from "react-router-dom"
import { useIsAuthed } from "./context"
import { Path as LoginPath } from "./login"

export const Route = ({ children, ...rest }) => {
  let isAuthed = useIsAuthed()
  return (
    <RouteBase
      {...rest}
      render={({ location }) =>
        isAuthed ? (
          children
        ) : (
          <Redirect
            to={{
              pathname: LoginPath,
              state: { from: location }
            }}
          />
        )
      }
    />
  )
}
