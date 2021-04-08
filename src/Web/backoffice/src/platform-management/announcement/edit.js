import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Stack, Box, Button, Text, Modal } from "bumbag"
import {
    Create as CreateMenu,
    Update as UpdateMenu,
    Verify as VerifyMenu,
    View as ViewMenu
} from "./menu"
import {
    useIsDisplayMenuByRoute,
    useUserBrandId,
    useUserLicenseeId,
    useUserResellerId,
    useUserRoleType
} from "../../user/context"
import { FieldValues } from "../../layout/Grid"
import Label from "../../form/Label"
import Disclosure from "../../dialog/Disclosure"
import {
    useAddAnnouncement,
    useEditAnnouncement,
    useVerifyAnnouncement
} from "./client"
import { Trans, useTrans } from "../../i18n"
import parseDate from "date-fns/fp/parseISO"
import { CancelText } from "../../button/ActionButtons"
import FormDialog from "../../form/FormDialog"
import Input from "../../input/Input"
import DateTimeInput from "../../date/DateTimeInput"
import { useForm, useFormContext } from "react-hook-form"
import { MerchantIdHookFormSelect } from "../../merchant/MerchantSelect"
import { nextRoleType } from "../../system/roles/type"
import Textarea from "../../textarea"

export const AddButton = ({ onSubmitted, ...props }) => {
    let dialog = Modal.useState()

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
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog, ...props }) => {
    let userBrandId = useUserBrandId()
    let [data, setSubmit] = useState()
    let { result, error, mutate, isLoading } = useAddAnnouncement(data)

    const onSubmit = ({ brandId = userBrandId, form, to, ...formData }) => {
        setSubmit({
            brandId,
            form: form ? form.toISOString() : form,
            to: to ? to.toISOString() : to,
            ...formData
        })
    }

    const clearResult = () => {
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "新增公告",
        en: "Add Announcement"
    })

    return (
        <FormDialog
            defaultValues={{
                subject: "",
                from: null,
                to: null,
                content: ""
            }}
            onSubmit={onSubmit}
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
            <EditBody isAdd={true} />
        </FormDialog>
    )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
    let dialog = Modal.useState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <Trans zh="修改" en="Edit" />
            </Disclosure>
            {dialog.visible && (
                <EditDialog
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
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
    let from = parseDate(record.from)
    let to = parseDate(record.to)

    let title = useTrans({
        zh: "查看公告",
        en: "View Announcement"
    })

    return (
        <FormDialog
            defaultValues={{
                ...record,
                from,
                to
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
            <VerifyBody readonly />
        </FormDialog>
    )
}

const EditDialog = ({
    record,
    onSubmitted,
    dialog,
    finalFocusRef,
    ...props
}) => {
    let { id } = record
    let [data, setSubmit] = useState()
    let { result, error, mutate, isLoading } = useEditAnnouncement(id, data)

    const onSubmit = ({ from, to, ...formData }) => {
        setSubmit({
            from: from?.toISOString(),
            to: to?.toISOString(),
            ...formData
        })
    }

    const clearResult = () => {
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "修改公告",
        en: "Edit Announcement"
    })

    return (
        <FormDialog
            defaultValues={{
                ...record,
                from: parseDate(record.from),
                to: parseDate(record.to)
            }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={clearResult}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            finalFocusRef={finalFocusRef}
            placement="top"
            css={DialogCss}
        >
            <EditBody />
        </FormDialog>
    )
}

const EditBody = ({ readonly = false, isAdd = false } = {}) => {
    let resellerId = useUserResellerId()
    let licenseeId = useUserLicenseeId()
    let brandId = useUserBrandId()
    let userRoleType = useUserRoleType()

    const { getValues } = useFormContext()
    let minDate = readonly ? undefined : getValues("from")
    let maxDate = readonly ? undefined : getValues("to")

    let required = !readonly
    let textareaHeight = readonly ? "15vh" : "30vh"
    return (
        <FieldValues>
            {isAdd && !brandId && (
                <Label required={required}>
                    <Trans zh="品牌" en="Brand" />
                </Label>
            )}
            {isAdd && !brandId && (
                <MerchantIdHookFormSelect
                    start={nextRoleType(userRoleType)}
                    resellerId={resellerId}
                    licenseeId={licenseeId}
                />
            )}

            <Label name="subject" required={required}>
                <Trans zh="主题" en="Subject" />
            </Label>
            <Input name="subject" required={required} disabled={readonly} />

            <Label name="from" required={required}>
                <Trans zh="开始时间" en="Start" />
            </Label>
            <Box>
                <DateTimeInput
                    name="from"
                    maxDate={maxDate}
                    required={required}
                    disabled={readonly}
                />
            </Box>

            <Label name="to" required={required}>
                <Trans zh="结束时间" en="End" />
            </Label>
            <Box>
                <DateTimeInput
                    name="to"
                    minDate={minDate}
                    required={required}
                    disabled={readonly}
                />
            </Box>

            <Label name="content" required={required}>
                <Trans zh="内容" en="Content" />
            </Label>
            <Textarea
                name="content"
                required={required}
                disabled={readonly}
                height={textareaHeight}
                css={css`
                    height: ${textareaHeight};

                    & .bb-FieldWrapper,
                    & .bb-TextareaWrapper,
                    & .bb-Textarea {
                        height: ${textareaHeight};
                    }
                `}
            />
        </FieldValues>
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

const DialogCss = css`
    min-width: 65vw;
    min-height: 65vh;

    & .bb-DialogContent {
        min-height: 65vh;
    }
`

const VerifyDialog = ({
    record,
    onSubmitted,
    dialog,
    finalFocusRef,
    ...props
}) => {
    let defaultValues = {
        ...record,
        from: parseDate(record.from),
        to: parseDate(record.to),
        remarks: ""
    }
    let formMethods = useForm({ defaultValues })

    let [type, setType] = useState()
    let [data, setSubmit] = useState()
    let { result, error, mutate, isLoading } = useVerifyAnnouncement(
        {
            id: record?.id,
            type
        },
        data
    )

    const onSubmit = ({ remarks }) => {
        let data = {
            remarks
        }
        setSubmit(data)
    }

    const clearResult = () => {
        setSubmit(null)
    }

    let title = useTrans({
        zh: "审核公告",
        en: "Verify Announcement"
    })

    return (
        <FormDialog
            formMethods={formMethods}
            onSubmit={onSubmit}
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
            <VerifyBody />
        </FormDialog>
    )
}

const VerifyBody = ({ readonly = false } = {}) => {
    return (
        <Stack spacing="major-2">
            <EditBody readonly={true} />
            <Box
                border="default"
                height="1px"
                width="100%"
                borderBottom="none"
            ></Box>

            <FieldValues>
                <Label name="remarks">
                    <Trans zh="备注" en="Remarks" />
                </Label>
                <Textarea
                    name="remarks"
                    disabled={readonly}
                    css={css`
                        & textarea {
                            resize: none;
                        }
                    `}
                />
            </FieldValues>
        </Stack>
    )
}
