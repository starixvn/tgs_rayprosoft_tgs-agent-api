import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Dialog, Text, Box, List } from "bumbag"
import { ViewDisclosure } from "../../dialog/Disclosure"
import { Locales, Trans, useTrans } from "../../i18n"
import { useGameProviderList } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"

export const GameProviderListDialogDisclosure = ({
    dialogTitle,
    gameProviderIds = [],
    ...props
}) => {
    let dialog = useModalState()

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <GameProviderListDialog
                    title={dialogTitle}
                    gameProviderIds={gameProviderIds}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const GameProviderListDialog = ({
    title = "",
    gameProviderIds = [],
    dialog
}) => {
    let { isLoading, data = {}, error } = useGameProviderList({
        showAll: true,
        sortby: "name"
    })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    let gameProviders =
        data?.items?.filter(({ id }) => gameProviderIds.includes(id)) ?? []

    return (
        <Dialog.Modal
            title={title}
            showCloseButton
            hideOnClickOutside={false}
            {...dialog}
        >
            <List overflow="auto">
                {gameProviders.map(({ name, code }, index) => (
                    <List.Item key={index}>{name}</List.Item>
                ))}
            </List>
            {gameProviders.length == 0 && (
                <Trans zh="无活动厂商" en="No Game Providers" />
            )}
        </Dialog.Modal>
    )
}
