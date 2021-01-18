import React, { useState, useEffect } from "react"
import { Global, css } from "@emotion/react"
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
  Alert,
  Modal,
  Icon,
  Flex
} from "bumbag"
import { useIsAuthed as useUserIsAuthed, useMenuCodes } from "./user/context"
import { Form as Login, Path as LoginPath } from "./user/login"
import { Route as AuthRouter } from "./user/auth"
import { AllMenus } from "./menu"
import { AllContents } from "./content"
import { useUsername, useDeauth } from "./user/context"
import { SWRConfig } from "swr"
import { Trans, useLocale, Locales, useEnglish, useChinese } from "./i18n"

function App() {
  let isUserAuthed = useUserIsAuthed()

  return (
    <SWRConfig
      value={{
        revalidateOnFocus: false,
        shouldRetryOnError: false
      }}
    >
      <Switch>
        <Route path={LoginPath}>
          <Login />
        </Route>
        <AuthRouter path="*">
          <Layout>
            <AllContents />
          </Layout>
        </AuthRouter>
      </Switch>
    </SWRConfig>
  )
}

const HeaderHeight = "60px"
const sidebarWidth = 250
const FooterHeight = "0px" //"30px"

const Layout = ({ children }) => {
  return (
    <PageWithHeader sticky header={<Header />} headerHeight={HeaderHeight}>
      <PageWithSidebar
        id="pageWithSidebar"
        sidebar={<Sidebar />}
        sidebarPlacement="left"
        sidebarWidth={`${sidebarWidth}px`}
        css={css`
          & .bb-PageWithSidebarSidebarExpandedWrapper {
            width: ${getSidebarWidth()}px;
          }

          & .bb-PageWithSidebarContent {
            height: 100%;
          }
        `}
      >
        <Box minHeight={`calc(100% - ${FooterHeight})`}>{children}</Box>
        {/* <Footer /> */}
      </PageWithSidebar>
    </PageWithHeader>
  )
}

export const getSidebarWidth = () => {
  let sidebarWrapper = document.querySelector(
    "#pageWithSidebar .bb-PageWithSidebarSidebarExpandedWrapper"
  )
  return sidebarWrapper?.offsetWidth ?? sidebarWidth
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
              <DropdownMenu.Item onClick={logout}>
                <Trans zh="登出" en="Logout" />
              </DropdownMenu.Item>
            }
          >
            <Button variant="ghost" iconAfter="chevron-down">
              {usename}
            </Button>
          </DropdownMenu>
        </TopNav.Item>
        <LangItem display="none" />
      </TopNav.Section>
    </TopNav>
  )
}

const LangItem = ({ ...props }) => {
  let locale = useLocale()
  let chinese = "中文"
  let english = "English"

  let useEn = useEnglish()
  let useZh = useChinese()
  return (
    <TopNav.Item {...props}>
      <DropdownMenu
        menu={
          <>
            <DropdownMenu.Item onClick={useZh}>{chinese}</DropdownMenu.Item>
            <DropdownMenu.Item onClick={useEn}>{english}</DropdownMenu.Item>
          </>
        }
      >
        <Button variant="ghost" iconAfter="chevron-down">
          {locale === Locales.zh && chinese}
          {locale === Locales.en && english}
        </Button>
      </DropdownMenu>
    </TopNav.Item>
  )
}

const Sidebar = () => {
  let { pathname: selectedId } = useLocation()

  return (
    <Stack spacing="major-1" paddingY="major-2">
      <SideNav selectedId={selectedId}>
        <AllMenus />
      </SideNav>
    </Stack>
  )
}

const Footer = () => {
  return (
    <Box textAlign="center" height={FooterHeight}>
      <Text fontWeight="500" fontSize="150">
        version {import.meta.env.SNOWPACK_PUBLIC_VERSION}
      </Text>
    </Box>
  )
}

export default App
