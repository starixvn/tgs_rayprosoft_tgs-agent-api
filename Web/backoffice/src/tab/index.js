import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import {
  useTabState as useReakitTabState,
  Tab as ReakitTab,
  TabList as ReakitTabList,
  TabPanel as ReakitTabPanel
} from "reakit/Tab"
import { Box, Tabs as BumbagTabs, useClassName } from "bumbag"

export const useTabState = useReakitTabState

export const Tabs = ({ use = Box, children, ...props }) => {
  return (
    <BumbagTabs use={use} {...props}>
      {children}
    </BumbagTabs>
  )
}

export const TabList = ({ use = TabList, children, ...props }) => {
  return (
    <BumbagTabs.List use={ReakitTabList} {...props}>
      {children}
    </BumbagTabs.List>
  )
}

export const Tab = ({ tabId, children, ...props }) => {
  let tabProps = BumbagTabs.Tab.useProps({ tabId })
  return (
    <ReakitTab {...tabProps} {...props}>
      {children}
    </ReakitTab>
  )
}

export const TabPanel = ({
  tabId,
  padding = "major-2",
  children,
  ...props
}) => {
  let panelProps = BumbagTabs.Panel.useProps({ tabId, padding })
  return (
    <ReakitTabPanel {...panelProps} {...props}>
      {children}
    </ReakitTabPanel>
  )
}
