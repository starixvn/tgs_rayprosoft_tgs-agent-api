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
    FieldWrapper
} from "bumbag"
import { useTrans } from "../i18n"
import { DetailDisclosure } from "../dialog/Disclosure"

export const LogDetailDialogDisclosure = ({ record, ...props }) => {
    let dialog = useModalState()

    return (
        <>
            <DetailDisclosure {...dialog} />
            {dialog.visible && <DetailDialog record={record} dialog={dialog} />}
        </>
    )
}

const DetailDialog = ({ record = {}, dialog, ...props }) => {
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
            maxWidth="70vw"
            {...dialog}
        >
            <FieldStack orientation="vertical">
                <Card variant="bordered" title="Request">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Code isBlock>{record.requestHeaders}</Code>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Code isBlock>{record.requestContent}</Code>
                        </FieldWrapper>
                    </FieldStack>
                </Card>

                <Card variant="bordered" title="Response">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Code isBlock>{record.responseHeaders}</Code>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Code isBlock>{record.responseContent}</Code>
                        </FieldWrapper>
                    </FieldStack>
                </Card>
            </FieldStack>
        </Dialog.Modal>
    )
}

export default DetailDialog
