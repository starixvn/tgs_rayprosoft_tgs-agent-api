import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Code,
    Card,
    FieldStack,
    FieldWrapper,
    Box
} from "bumbag"
import { useTrans } from "../i18n"
import { DetailDisclosure } from "../dialog/Disclosure"

export const LogExceptionDialogDisclosure = ({ value, ...props }) => {
    let dialog = useModalState()

    if (!value) {
        return "-"
    }

    return (
        <>
            <DetailDisclosure {...dialog} />
            {dialog.visible && (
                <ExceptionDialog value={value} dialog={dialog} />
            )}
        </>
    )
}

const ExceptionDialog = ({ value = "", dialog, ...props }) => {
    const title = useTrans({
        zh: "详情",
        en: "Detail"
    })

    return (
        <Dialog.Modal
            title={title}
            showActionButtons={false}
            hideOnClickOutside={false}
            showCloseButton
            width="60vw"
            maxWidth="80vw"
            maxHeight="90vh"
            css={css`
                & .bb-DialogContent {
                    max-height: 90vh;
                }
            `}
            {...dialog}
        >
            <Code isBlock wordBreak="break-word">
                {value}
            </Code>
        </Dialog.Modal>
    )
}
