import React, { useState, useEffect } from "react"
import { css } from "@emotion/react"
import {
    Table,
    Set,
    Button,
    Box,
    Stack,
    PageWithHeader,
    TopNav,
    Popover,
    Input,
    InputField,
    FieldStack,
    Text,
    FieldWrapper,
    DropdownMenu,
    Modal,
    Dialog
} from "bumbag"
import {
    Switch,
    Route,
    Link,
    useHistory,
    useParams,
    useRouteMatch
} from "react-router-dom"
import { useBrandToken } from "../client/brand"
import {
    State,
    useCurrency,
    usePlayerType,
    useSetCurrency,
    useSetPlayerType
} from "./State"
import CurrencyDropdownMenu from "../currency/DropdownMenu"
import { CreatePlayerDialog } from "./CreatePlayerDialog"
import { PlayerTypeDropdownMenu } from "./player/PlayerType"
import { AnnouncementDialog } from "./AnnouncementDialog"

export const BrandHeader = ({ navItems = [], onPlayerCreated }) => {
    let currency = useCurrency()
    let setCurrency = useSetCurrency()

    let playerType = usePlayerType()
    let setPlayerType = useSetPlayerType()

    let { brandCode } = useParams()
    let { token = "", expires } = useBrandToken({ brandCode, currency })

    let now = new Date().getTime()
    let isExpired = now > expires

    return (
        <TopNav padding="major-1">
            <TopNav.Section>
                <TopNav.Item>
                    <BrandDropdownMenu
                        brandCode={brandCode}
                        currency={currency}
                        onPlayerCreated={onPlayerCreated}
                    />
                </TopNav.Item>
                <TopNav.Item>
                    <CurrencyDropdownMenu
                        value={currency}
                        onChange={setCurrency}
                    />
                </TopNav.Item>
                <TopNav.Item>
                    <PlayerTypeDropdownMenu
                        value={playerType}
                        onChange={setPlayerType}
                    />
                </TopNav.Item>
                {navItems}
            </TopNav.Section>
        </TopNav>
    )
}

const BrandDropdownMenu = ({ brandCode, currency, onPlayerCreated }) => {
    let createPlayerDialog = Modal.useState()
    let announcementDialog = Modal.useState()

    return (
        <>
            <DropdownMenu
                menu={
                    <>
                        <DropdownMenu.Item onClick={createPlayerDialog.show}>
                            Create Player
                        </DropdownMenu.Item>
                        <DropdownMenu.Item onClick={announcementDialog.show}>
                            Announcements
                        </DropdownMenu.Item>
                    </>
                }
            >
                <Button iconAfter="chevron-down">{brandCode}</Button>
            </DropdownMenu>
            {createPlayerDialog.visible && (
                <CreatePlayerDialog
                    brandCode={brandCode}
                    currency={currency}
                    onPlayerCreated={onPlayerCreated}
                    dialog={createPlayerDialog}
                />
            )}

            {announcementDialog.visible && (
                <AnnouncementDialog
                    brandCode={brandCode}
                    currency={currency}
                    dialog={announcementDialog}
                />
            )}
        </>
    )
}

export default BrandHeader
