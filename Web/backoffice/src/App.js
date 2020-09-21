import React, { useState, useEffect } from "react"
import { Global, css } from "@emotion/core"
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
import logo from "./logo.svg"
import {
  Container,
  PageContent,
  PageWithHeader,
  PageWithSidebar,
  TopNav,
  SideNav,
  Stack,
  Box,
  DropdownMenu,
  Button,
  Text,
  Alert
} from "bumbag"
import { useIsAuthed as useUserIsAuthed, useMenuCodes } from "./user/context"
import { Form as Login, Path as LoginPath } from "./user/login"
import { Route as AuthRouter } from "./user/auth"
import {
  Code as SystemCode,
  Menu as SystemMenu,
  Content as SystemContent
} from "./system"
import PageNotFound from "./page/NotFound"
import { useUsername, useDeauth } from "./user/context"

function App() {
  let isUserAuthed = useUserIsAuthed()
  //return isUserAuthed ? <AuthedLayout>TODO</AuthedLayout> : <Login></Login>
  return (
    <Switch>
      <Route path={LoginPath}>
        <Login />
      </Route>
      <AuthRouter path="*">
        <Layout>
          <AuthContens />
        </Layout>
      </AuthRouter>
    </Switch>
  )
}

const Layout = ({ children }) => {
  return (
    <PageWithHeader
      sticky
      header={<Header />}
      css={css`
        min-height: 100%;
      `}
    >
      <PageWithSidebar
        sidebar={<Sidebar />}
        sidebarPlacement="left"
        sidebarWidth="270px"
        css={css``}
      >
        {children}
        <PageContent>
          <Footer />
        </PageContent>
      </PageWithSidebar>
    </PageWithHeader>
  )
}

const AuthContens = () => {
  let codes = useMenuCodes()

  return (
    <Switch>
      {codes.map((code) => {
        let path = `/${code}`

        return (
          <AuthRouter key={code} path={path}>
            <Content code={code} />
          </AuthRouter>
        )
      })}
      <AuthRouter path="*">
        <PageContent></PageContent>
      </AuthRouter>
    </Switch>
  )
}

const Content = ({ code }) => {
  switch (code) {
    case SystemCode:
      return <SystemContent />
    default:
      return <PageNotFound />
  }
}

const Header = () => {
  let usename = useUsername()
  let deauth = useDeauth()

  const logout = () => {
    deauth()
    location.replace(LoginPath)
  }

  return (
    <TopNav>
      <TopNav.Section>
        <TopNav.Item href="/" fontWeight="semibold"></TopNav.Item>
      </TopNav.Section>
      <TopNav.Section marginRight="major-2">
        <TopNav.Item>
          <DropdownMenu
            menu={
              <DropdownMenu.Item onClick={logout}>Logout</DropdownMenu.Item>
            }
          >
            <Button variant="ghost" iconAfter="chevron-down">
              {usename}
            </Button>
          </DropdownMenu>
        </TopNav.Item>
      </TopNav.Section>
    </TopNav>
  )
}

const Sidebar = () => {
  let { pathname: selectedId } = useLocation()
  let codes = useMenuCodes()

  return (
    <Stack spacing="major-1" paddingY="major-2">
      <SideNav selectedId={selectedId}>
        {codes.map((code) => (
          <MenuItem key={code} code={code} />
        ))}
      </SideNav>
    </Stack>
  )
}

const MenuItem = ({ code }) => {
  switch (code) {
    case SystemCode:
      return <SystemMenu />
    default:
      return null
  }
}

const Footer = () => {
  return (
    <Box textAlign="center">
      <Text fontWeight="500" fontSize="150">
        version {import.meta.env.SNOWPACK_PUBLIC_VERSION}
      </Text>
    </Box>
  )
}

export default App
