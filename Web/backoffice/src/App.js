import React, { useState } from "react"
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
  Text
} from "bumbag"
import { Form } from "./auth/login"

function App() {
  return (
    <PageWithHeader sticky header={<Header />}>
      <PageWithSidebar
        sidebar={<Sidebar />}
        sidebarPlacement="left"
        sidebarWidth="270px"
        css={css``}
      >
        <PageContent>
          <Form></Form>
        </PageContent>
        <PageContent>
          <Footer />
        </PageContent>
      </PageWithSidebar>
    </PageWithHeader>
  )
}

const Header = () => {
  return (
    <TopNav>
      <TopNav.Section>
        <TopNav.Item href="/" fontWeight="semibold">
          App Name
        </TopNav.Item>
      </TopNav.Section>
      <TopNav.Section marginRight="major-2">
        <TopNav.Item>
          <DropdownMenu
            menu={<DropdownMenu.Item>TODO: Logout</DropdownMenu.Item>}
          >
            <Button variant="ghost" iconAfter="chevron-down">
              TODO: Username
            </Button>
          </DropdownMenu>
        </TopNav.Item>
      </TopNav.Section>
    </TopNav>
  )
}

const Sidebar = () => {
  return (
    <Stack spacing="major-1" paddingY="major-2">
      <SideNav>
        <SideNav.Level title="System">
          <SideNav.Item href="#" navId="menuItem">
            TODO: sub menu
          </SideNav.Item>
        </SideNav.Level>
      </SideNav>
    </Stack>
  )
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
