import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Box,
    Set,
    FieldStack,
    FieldWrapper
} from "bumbag"
import Disclosure from "../../dialog/Disclosure"
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import { useApiLog } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"

export const ApiLogDetailDialogDisclosure = ({ trackingId, ...props }) => {
    let dialog = useModalState()

    return (
        <>
            <Disclosure>
                <Trans zh="Detail" en="Detail" />
            </Disclosure>
            {dialog.visible && (
                <DetailDialog trackingId={trackingId} dialog={dialog} />
            )}
        </>
    )
}

const DetailDialog = ({ trackingId, dialog, ...props }) => {
    let { data, isLoading, error } = useApiLog(trackingId)

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog dialog={dialog} onClickClose={dialog.hide} />
    }

    return (
        <Dialog.Modal
            title="Log"
            showActionButtons={false}
            showCloseButton
            {...dialog}
        >
            <FieldStack orientation="vertical">
                <Card variant="bordered" title="Request">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Text.Block>{data.requestHeaders}</Text.Block>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Text.Block>{data.requestContent}</Text.Block>
                        </FieldWrapper>
                    </FieldStack>
                </Card>

                <Card variant="bordered" title="Response">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Text.Block>{data.responseHeaders}</Text.Block>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Text.Block>{data.responseContent}</Text.Block>
                        </FieldWrapper>
                    </FieldStack>
                </Card>
            </FieldStack>
        </Dialog.Modal>
    )
}
