import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Dialog } from "bumbag"
import { View as ViewMenu, Update as UpdateMenu } from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import {
    Tab as PlayerTab,
    Panel as PlayerPanel,
    EditPanel as PlayerEditTab,
    Key as PlayerId
} from "./player"
import {
    Tab as ContractTab,
    Panel as ContractPanel,
    EditPanel as ContractEditPanel
} from "./contact"
import { Tab as BankTab, Panel as BankPanel } from "./bank"
import { Tab as BettingTab, Panel as BettingPanel } from "./betting"
import { Tab as OperationTab, Panel as OperationPanel } from "./operation"
import { Tab as PasswordTab, Panel as PasswordPanel } from "./password"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import { usePlayer } from "./client"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import Disclosure from "../../dialog/Disclosure"
import { Modal, useModalState } from "../../dialog/Modal"
import { Tabs, TabList, useTabState } from "../../tab"
import {
    PlayerBalanceChangePanel,
    PlayerBalanceChangeTab
} from "./PlayerBalanceChange"

export const ViewButton = ({ player, brandId, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="查看" en="View" />
            </Disclosure>
            {dialog.visible && (
                <TabsDialog
                    playerCode={player.playerCode}
                    brandId={brandId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

export const EditButton = ({ brandId, player, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="修改" en="Edit" />
            </Disclosure>
            {dialog.visible && (
                <TabsDialog
                    edit={true}
                    brandId={brandId}
                    playerCode={player.playerCode}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const TabsDialog = ({
    playerCode,
    brandId,
    edit = false,
    dialog,
    ...props
}) => {
    const readonly = !edit
    let tab = useTabState({
        selectedId: PlayerId
    })

    let title = useTrans({
        zh: "玩家详情",
        en: "Player info"
    })

    const { data, isLoading, isError, error, mutate } = usePlayer(playerCode)

    const onSubmitted = () => {
        props?.onSubmitted()
        dialog.hide()
        mutate()
    }

    const onBankSubmitted = () => {
        mutate()
    }

    const onClickClose = dialog.hide

    return (
        <>
            {isLoading && <LoadingBackdrop />}
            {isError && <AlertDialog {...error} onClickClose={onClickClose} />}
            {data && (
                <Dialog.Modal
                    title={title}
                    hideOnClickOutside={false}
                    showCloseButton={true}
                    placement="top"
                    {...dialog}
                    css={css`
                        min-width: 80vw;
                        min-height: 85vh;

                        & .bb-DialogContent {
                            min-height: 85vh;
                        }
                    `}
                >
                    {readonly && (
                        <ViewTabs
                            playerCode={playerCode}
                            brandId={brandId}
                            player={data}
                            tab={tab}
                        />
                    )}
                    {edit && (
                        <EditTabs
                            playerCode={playerCode}
                            brandId={brandId}
                            player={data}
                            tab={tab}
                            onSubmitted={onSubmitted}
                            onBankSubmitted={onBankSubmitted}
                            onClickClose={onClickClose}
                        />
                    )}
                </Dialog.Modal>
            )}
        </>
    )
}

const ViewTabs = ({ playerCode, brandId, player, tab }) => {
    return (
        <Tabs isFitted>
            <TabList use={TabList} {...tab}>
                <PlayerTab {...tab} />
                <ContractTab {...tab} />
                <BankTab {...tab} />
                <BettingTab {...tab} />
                <OperationTab {...tab} />
                <PlayerBalanceChangeTab {...tab} />
            </TabList>
            <PlayerPanel player={player} {...tab} />
            <ContractPanel player={player} {...tab} />
            <BankPanel banks={player.banks} {...tab} />
            <BettingPanel
                playerCode={playerCode}
                player={player}
                brandId={brandId}
                {...tab}
            />
            <OperationPanel playerCode={playerCode} {...tab} />
            <PlayerBalanceChangePanel
                playerCode={playerCode}
                brandId={brandId}
                {...tab}
            />
        </Tabs>
    )
}

const EditTabs = ({
    brandId,
    playerCode,
    player,
    tab,
    onSubmitted,
    onBankSubmitted,
    onClickClose
}) => {
    return (
        <Tabs isFitted>
            <TabList use={TabList} {...tab}>
                <PlayerTab {...tab} />
                <ContractTab {...tab} />
                <BankTab {...tab} />
                <PasswordTab {...tab} />
            </TabList>
            <PlayerEditTab
                player={player}
                brandId={brandId}
                playerCode={playerCode}
                onSubmitted={onSubmitted}
                onClickClose={onClickClose}
                {...tab}
            />
            <ContractEditPanel
                player={player}
                playerCode={playerCode}
                onSubmitted={onSubmitted}
                onClickClose={onClickClose}
                {...tab}
            />
            <BankPanel
                playerCode={playerCode}
                banks={player.banks}
                edit={true}
                onSubmitted={onBankSubmitted}
                {...tab}
            />
            <PasswordPanel
                playerCode={playerCode}
                onSubmitted={onSubmitted}
                onClickClose={onClickClose}
                {...tab}
            />
        </Tabs>
    )
}
