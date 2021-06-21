import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller, useWatch } from "react-hook-form"
import {
    Stack,
    Box,
    Button,
    Text,
    Modal,
    Set,
    Popover,
    List,
    useModalState,
    Flex
} from "bumbag"
import {
    Create as CreateMenu,
    Verify as VerifyMenu,
    View as ViewMenu
} from "./menu"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"
import { Trans, useTrans, useLocale, Locales } from "../../i18n"
import {
    useAddStationLetter,
    useStationLetterUpload,
    useVerifyStationLetter
} from "./client"
import { CancelText } from "../../button/ActionButtons"
import {
    AllPlayers,
    DesignatedPlayers,
    PlayerTargetSelect
} from "./PlayerTargetSelect"
import Alert from "../../dialog/Alert"
import Input from "../../input/Input"
import FormDialog from "../../form/FormDialog"
import Textarea from "../../textarea"
import { toFileUploadPath } from "../../file/upload/FileUploadPath"
import { HtmlEditor } from "../../text/editor/HtmlEditor"

export const AddButton = ({ brandId, onSubmitted, ...props }) => {
    let dialog = useModalState()
    let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="新增" en="Add" />
            </Disclosure>
            {dialog.visible && (
                <AddDialog
                    brandId={brandId}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const AddDialog = ({ brandId, onSubmitted, dialog, ...props }) => {
    let [data, setSubmit] = useState()
    let { result, isLoading, error } = useAddStationLetter(data)

    const clearResult = () => {
        setSubmit(null)
    }

    let title = useTrans({
        zh: "新增站内信",
        en: "Add Station Letter"
    })

    return (
        <FormDialog
            defaultValues={{
                brandId,
                target: AllPlayers,
                subject: "",
                content: ""
            }}
            onSubmit={({ target, ...data }) => {
                if (target === AllPlayers) {
                    data.assignedAllPlayers = true
                }
                setSubmit({
                    ...data,
                    brandId
                })
            }}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={clearResult}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            placement="top"
            css={DialogCss}
        >
            <EditBody />
        </FormDialog>
    )
}

const DialogCss = css`
    min-width: 80vw;
    max-height: 90vh;

    & .bb-DialogContent {
        max-height: 80vh;
    }

    .tox-fullscreen & {
        top: 0;
        left: 0;
        transform: none;
    }
`

const EditBody = ({ readOnly = false } = {}) => {
    let { setValue, control } = useFormContext()
    let target = useWatch({
        control,
        name: "target"
    })

    let PlayerUsernamesFieldName = "playerUsernames"
    let playerUsernames = useWatch({
        control,
        name: PlayerUsernamesFieldName
    })

    let required = !readOnly
    let uploadInputRef = useRef()

    let [formData, setFormData] = useState()
    let { result, isLoading, error, deleteCache } =
        useStationLetterUpload(formData)

    const resetFormData = () => {
        if (uploadInputRef?.current) {
            uploadInputRef.current.value = ""
        }
        deleteCache()
        setFormData(undefined)
    }

    useEffect(() => {
        if (Array.isArray(result?.players) && !error) {
            setValue(
                "playerUsernames",
                result?.players.map((item) => item.username)
            )
        }
    }, [formData, result])

    return (
        <FieldValues>
            <Label name="target" required={required}>
                <Trans zh="对象" en="Target" />
            </Label>
            <Set>
                <PlayerTargetSelect required={required} disabled={readOnly} />
                {!readOnly && target === DesignatedPlayers && (
                    <Controller
                        control={control}
                        name={PlayerUsernamesFieldName}
                        shouldUnregister={true}
                        render={({
                            field: { onChange, onBlur, value, name, ref },
                            fieldState: { invalid, isTouched, isDirty }
                        }) => (
                            <>
                                <input
                                    ref={uploadInputRef}
                                    type="file"
                                    onChange={(event) => {
                                        let [file] = event.target.files
                                        if (file) {
                                            let formData = new FormData()
                                            formData.append(
                                                "file",
                                                file,
                                                file.name
                                            )
                                            setFormData(formData)
                                        } else {
                                            resetFormData()
                                        }
                                    }}
                                    css={css`
                                        display: none;
                                    `}
                                />
                                <Button
                                    type="button"
                                    palette="primary"
                                    size="small"
                                    onClick={(event) => {
                                        uploadInputRef?.current.click()
                                    }}
                                    isLoading={isLoading}
                                    disabled={isLoading}
                                >
                                    <Trans zh="上传文件" en="Upload file" />
                                </Button>
                                <Button
                                    use="a"
                                    href="/download/payments/list.xlsx"
                                    download="players.xlsx"
                                    palette="primary"
                                    size="small"
                                >
                                    <Trans
                                        zh="下载文件格式"
                                        en="Download Format File"
                                    />
                                </Button>
                                {error && (
                                    <Alert
                                        {...error}
                                        onClickClose={resetFormData}
                                    />
                                )}
                            </>
                        )}
                    />
                )}
                {readOnly &&
                    target === DesignatedPlayers &&
                    playerUsernames.length > 0 && (
                        <Popover.State>
                            <Popover.Disclosure
                                use={Button}
                                palette="primary"
                                background="primary"
                            >
                                <Trans zh="玩家列表" en="Players" />
                            </Popover.Disclosure>
                            <Popover hasArrow>
                                <List maxHeight="15em" overflow="auto">
                                    {playerUsernames.map((name, index) => (
                                        <List.Item key={index}>
                                            {name}
                                        </List.Item>
                                    ))}
                                </List>
                            </Popover>
                        </Popover.State>
                    )}
            </Set>

            <Label name="subject" required={required}>
                <Trans zh="主题" en="Subject" />
            </Label>
            <Input name="subject" required={required} disabled={readOnly} />

            <Label name="content" required={required} flex="1 auto">
                <Trans zh="内容" en="Content" />
            </Label>
            <HtmlEditor
                name="content"
                imagesUploadUrl={toFileUploadPath("stationLetter")}
                readOnly={readOnly}
            />
        </FieldValues>
    )
}

export const ViewButton = ({ record, ...props }) => {
    let dialog = Modal.useState()

    let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="查看" en="View" />
            </Disclosure>
            {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
        </>
    )
}

const ViewDialog = ({ record, dialog, ...props }) => {
    let title = useTrans({
        zh: "查看站内信",
        en: "View Station Letter"
    })

    return (
        <FormDialog
            defaultValues={{
                ...record,
                target: record.assignedAllPlayers
                    ? AllPlayers
                    : DesignatedPlayers
            }}
            title={title}
            placement="top"
            css={DialogCss}
            dialog={dialog}
            showActionButtons={false}
            footer={
                <Box alignX="right" width="100%">
                    <Button onClick={dialog.hide}>
                        <CancelText />
                    </Button>
                </Box>
            }
        >
            <VerifyBody readOnly={true} dialog={dialog} />
        </FormDialog>
    )
}

export const VerifyButton = ({ record, onSubmitted, ...props }) => {
    let dialog = Modal.useState()

    let isDisplay = useIsDisplayMenuByRoute([VerifyMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="审核" en="Verify" />
            </Disclosure>
            {dialog.visible && (
                <VerifyDialog
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const VerifyDialog = ({ record, onSubmitted, dialog, ...props }) => {
    let defaultValues = {
        ...record,
        target: record.assignedAllPlayers ? AllPlayers : DesignatedPlayers,
        remarks: ""
    }
    let formMethods = useForm({ defaultValues })

    let [type, setType] = useState()
    let [data, setSubmit] = useState()
    let { result, error, mutate, isLoading } = useVerifyStationLetter({
        id: record?.id,
        type,
        data
    })

    const clearResult = () => {
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "审核站内信",
        en: "Verify Station Letter"
    })

    return (
        <FormDialog
            formMethods={formMethods}
            onSubmit={setSubmit}
            onClickSubmit={(event) => {
                setType("pass")
            }}
            addonButtons={
                <Button
                    type="submit"
                    onClick={() => {
                        setType("reject")
                    }}
                    palette="danger"
                    variant="outlined"
                >
                    <Trans zh="拒绝" en="Reject" />
                </Button>
            }
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={clearResult}
            onSubmitted={onSubmitted}
            title={title}
            placement="top"
            css={DialogCss}
            dialog={dialog}
        >
            <VerifyBody dialog={dialog} readOnly={false} />
        </FormDialog>
    )
}

const VerifyBody = ({ readOnly = false, dialog }) => {
    return (
        <Stack spacing="major-2">
            <EditBody readOnly={true} dialog={dialog} />
            <Box border="default" height="1px" width="100%"></Box>
            <FieldValues>
                <Label name="remarks">
                    <Trans zh="备注" en="Remarks" />
                </Label>
                <Textarea name="remarks" disabled={readOnly} />
            </FieldValues>
        </Stack>
    )
}
