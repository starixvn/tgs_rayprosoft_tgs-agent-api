import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Code,
    FieldStack,
    FieldWrapper
} from "bumbag"
import Disclosure, { DetailDisclosure } from "../../dialog/Disclosure"
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import { useAdminLog } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"

export const AdminLogDetailDialogDisclosure = ({ id, ...props }) => {
    let dialog = useModalState()

    return (
        <>
            <DetailDisclosure {...dialog} />
            {dialog.visible && <DetailDialog id={id} dialog={dialog} />}
        </>
    )
}

const DetailDialog = ({ id, dialog, ...props }) => {
    let { data, isLoading, error } = useAdminLog(id)

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return (
            <AlertDialog
                {...error}
                dialog={dialog}
                onClickClose={dialog.hide}
            />
        )
    }

    return (
        <Dialog.Modal
            title="Admin Log"
            showActionButtons={false}
            showCloseButton
            {...dialog}
        >
            <FieldStack orientation="vertical">
                <Card variant="bordered" title="Request">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Code>{data.requestHeaders}</Code>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Code>{data.requestContent}</Code>
                        </FieldWrapper>
                    </FieldStack>
                </Card>

                <Card variant="bordered" title="Response">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Code>{data.responseHeaders}</Code>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Code>{data.responseContent}</Code>
                        </FieldWrapper>
                    </FieldStack>
                </Card>
            </FieldStack>
        </Dialog.Modal>
    )
}
