import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import {
    useTabState as useReakitTabState,
    Tab as BaseTab,
    TabList as BaseTabList,
    TabPanel as BaseTabPanel
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
        <BumbagTabs.List use={BaseTabList} {...props}>
            {children}
        </BumbagTabs.List>
    )
}

export const Tab = ({ tabId, type = "button", children, ...props }) => {
    let tabProps = BumbagTabs.Tab.useProps({ tabId })
    return (
        <BaseTab {...tabProps} type={type} {...props}>
            {children}
        </BaseTab>
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
        <BaseTabPanel {...panelProps} {...props}>
            {children}
        </BaseTabPanel>
    )
}
