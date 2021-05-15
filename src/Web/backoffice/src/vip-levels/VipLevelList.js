import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Dialog, Text, Box, List } from "bumbag"
import { ViewDisclosure } from "../dialog/Disclosure"
import { Locales, Trans, useTrans } from "../i18n"
import { useVipLevelList } from "./client"
import LoadingBackdrop from "../dialog/LoadingBackdrop"
import AlertDialog from "../dialog/Alert"
import { VipLevelLabel } from "./VipLevel"

export const VipLevelListDialogDisclosure = ({
    dialogTitle,
    vipLevels = [],
    ...props
}) => {
    let dialog = useModalState()

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <VipLevelListDialog
                    title={dialogTitle}
                    vipLevels={vipLevels}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const VipLevelListDialog = ({ title = "", vipLevels = [], dialog }) => {
    return (
        <Dialog.Modal
            title={title}
            showCloseButton
            hideOnClickOutside={false}
            {...dialog}
        >
            <List overflow="auto">
                {vipLevels.map((vipLevel, index) => (
                    <List.Item key={index}>
                        <VipLevelLabel value={vipLevel} />
                    </List.Item>
                ))}
            </List>
        </Dialog.Modal>
    )
}
