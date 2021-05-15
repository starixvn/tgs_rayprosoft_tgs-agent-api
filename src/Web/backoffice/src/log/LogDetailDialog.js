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
import { JsonEditor } from "../text/editor/JsonEditor"
import { tryStringifyJSON } from "../text/json"

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

    let [isRequestJson, requestContent = ""] = tryStringifyJSON(
        record.requestContent
    )

    let [isResponseJson, responseContent = ""] = tryStringifyJSON(
        record.responseContent
    )
    return (
        <Dialog.Modal
            title={title}
            showActionButtons={false}
            hideOnClickOutside={false}
            showCloseButton
            width="80vw"
            maxWidth="80vw"
            maxHeight="90vh"
            css={css`
                & .bb-DialogContent {
                    max-height: 90vh;
                }
            `}
            {...dialog}
        >
            <FieldStack orientation="vertical">
                <Card variant="bordered" title="Request">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Code isBlock>{record.requestHeaders}</Code>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Box>
                                {isRequestJson && (
                                    <JsonEditor
                                        height="30vh"
                                        value={requestContent}
                                        disabled
                                    />
                                )}
                                {!isRequestJson && (
                                    <Code isBlock wordBreak="break-word">
                                        {requestContent}
                                    </Code>
                                )}
                            </Box>
                        </FieldWrapper>
                    </FieldStack>
                </Card>

                <Card variant="bordered" title="Response">
                    <FieldStack>
                        <FieldWrapper label="Headers">
                            <Code isBlock>{record.responseHeaders}</Code>
                        </FieldWrapper>
                        <FieldWrapper label="Content">
                            <Box>
                                {isResponseJson && (
                                    <JsonEditor
                                        height="30vh"
                                        value={responseContent}
                                        disabled
                                    />
                                )}
                                {!isResponseJson && (
                                    <Code isBlock wordBreak="break-word">
                                        {responseContent}
                                    </Code>
                                )}
                            </Box>
                        </FieldWrapper>
                    </FieldStack>
                </Card>
            </FieldStack>
        </Dialog.Modal>
    )
}

export default DetailDialog
