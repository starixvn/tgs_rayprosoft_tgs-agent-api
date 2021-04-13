import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    useForm,
    FormProvider,
    useFormContext,
    Controller
} from "react-hook-form"
import {
    Stack,
    Box,
    Set,
    Button,
    Text,
    Modal,
    Dialog,
    Columns,
    Heading,
    Flex,
    Group,
    List as _List,
    applyTheme
} from "bumbag"
import { useModalState } from "../../dialog/Modal"
import { useIsDisplayMenuByRoute } from "../../user/context"
import { Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import { EditDisclosure } from "../../dialog/Disclosure"
import { ConfirmButtonFooter } from "../../form/ConfirmButtonFooter"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel as Label } from "../../form/Label"
import { BrandPaymentStatusToggleButton } from "./status"
import { useUpdatePayment, useBrandPaymentUpload } from "./client"
import AlertDialog from "../../dialog/Alert"
import {
    AllPlayerText,
    AccumulatedDepositText,
    UnlimitedText,
    IdentityAuthenticationText,
    PhoneAuthenticationText,
    VipLevelsText,
    PlayerListConditionRadioGroup
} from "./condition"
import { VipLevelCheckboxGroup } from "../../vip-levels/CheckboxGroup"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { NumberRangeInputGroup } from "../../number/RangeInput"
import FormDialog from "../../form/FormDialog"
import Textarea from "../../textarea"
import Input from "../../input/Input"
import { Checkbox } from "../../checkbox/Checkbox"
import { Switch } from "../../switch/Switch"
import clone from "clone-deep"

export const EditButton = ({ record, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} />
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

const EditDialog = ({ record = {}, onSubmitted, dialog, ...props }) => {
    let title = useTrans({
        zh: "编辑",
        en: "Edit"
    })

    /*
    let {
        data: brandPaymentData = {},
        isLoading: isLoadingPayment,
        error: getPaymentDataError,
        deleteCache: deleteGetPaymentCache
    } = useBrandPayment(record?.id)
    */

    let [submitData, setSubmit] = useState()

    let { result, isLoading, error, mutate, deleteCache } = useUpdatePayment(
        record.id,
        submitData
    )
    /*
    if (isLoadingPayment) {
        return <LoadingBackdrop {...dialog} />
    }

    if (getPaymentDataError) {
        return (
            <AlertDialog {...getPaymentDataError} onClickClose={dialog.hide} />
        )
    }
    */

    return (
        <FormDialog
            defaultValues={clone({
                ...record,
                reason: ""
            })}
            onSubmit={(data) => {
                setSubmit({
                    ...record,
                    /*
                    ...brandPaymentData.payment,
                    */
                    ...data,
                    depositedAmountMax:
                        data?.depositedAmountMax ?? record?.depositedAmountMax
                })
            }}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={() => {
                deleteCache()
                mutate(undefined, false)
                setSubmit(undefined)
            }}
            onSubmitted={() => {
                // deleteGetPaymentCache()
                onSubmitted?.()
            }}
            footer={
                <ConfirmButtonFooter
                    alignX="center"
                    width="100%"
                    isLoading={isLoading}
                    disabled={isLoading}
                />
            }
            title={title}
            showCloseButton
            showActionButtons={false}
            maxWidth="unset"
            width="80vw"
            height="95vh"
            maxHeight="95vh"
            placement="top"
            display="flex"
            flexDirection="column"
            css={css`
                & .bb-Flex.bb-DialogContent {
                    max-height: 95vh;
                    flex: 1 auto;
                    padding-bottom: 0.5em;
                }

                & .bb-DialogContent > .bb-Box {
                    display: flex;
                    flex-direction: column;
                }
            `}
            dialog={dialog}
        >
            <EditBody record={record} />
        </FormDialog>
    )
}

const EditBody = ({ record = {}, dialog, ...props }) => {
    let { watch, setValue } = useFormContext()
    let transferLimitTo = watch("transferLimitTo")
    let transferLimitFrom = watch("transferLimitFrom")

    return (
        <Stack
            orientation="vertical"
            display="flex"
            flexDirection="column"
            flex="1 auto"
        >
            <Columns flex="1 auto" marginBottom="major-1">
                <Columns.Column>
                    <Stack
                        spacing="major-1"
                        display="flex"
                        flexDirection="column"
                        height="100%"
                    >
                        <Heading use="h5">
                            <Trans zh="基本资料" en="Basic" />
                        </Heading>
                        <FieldValues>
                            <Label>
                                <Trans zh="支付厂商" en="Payment Vendor" />
                            </Label>
                            <ValueBlock>{record?.paymentVendorName}</ValueBlock>

                            <Label>
                                <Trans zh="支付方式" en="Payment Medthod" />
                            </Label>
                            <ValueBlock>{record?.paymentMethodName}</ValueBlock>

                            <Label>
                                <Trans zh="支付通道" en="Payment Channel" />
                            </Label>
                            <ValueBlock>
                                {record?.paymentChannelName}
                            </ValueBlock>

                            <Label required>
                                <Trans zh="通道名称" en="Channel Name" />
                            </Label>
                            <Input name="channelName" size="small" required />

                            <Label>
                                <Trans zh="状态" en="Status" />
                            </Label>
                            <Box>
                                <BrandPaymentStatusToggleButton name="paymentStatus" />
                            </Box>

                            <Label required>
                                <Trans zh="单笔限额" en="Limit" />
                            </Label>
                            <NumberRangeInputGroup
                                names={["transferLimitFrom", "transferLimitTo"]}
                                required
                                min={[0, transferLimitFrom]}
                                max={[transferLimitTo]}
                                width="100%"
                            />
                        </FieldValues>

                        <Columns flex="1 auto">
                            <UploadListColumn
                                name="whitelist"
                                title={<Trans zh="白名单列表" en="Whitelist" />}
                                list={record.whitelist}
                                fileName="whitelist.xlsx"
                            />
                            <UploadListColumn
                                name="blacklist"
                                title={<Trans zh="黑名单列表" en="Blacklist" />}
                                list={record.blacklist}
                                fileName="blacklist.xlsx"
                            />
                        </Columns>
                    </Stack>
                </Columns.Column>
                <Columns.Column>
                    <Heading use="h5">
                        <Trans zh="开放条件" en="Conditions" />
                    </Heading>
                    <FieldValues>
                        <Label name="playerListCondition">
                            <Trans zh="符合条件" en="Eligible" />
                        </Label>
                        <Box alignY="center">
                            <PlayerListConditionRadioGroup name="playerListCondition" />
                        </Box>

                        <Label name="allPlayer">
                            <AllPlayerText />
                        </Label>
                        <Box alignY="center">
                            <Switch name="allPlayers" width="auto" />
                        </Box>

                        <Label required>
                            <AccumulatedDepositText />
                        </Label>
                        <Box alignY="center">
                            <AccumulatedDepositGroup />
                        </Box>

                        <Label>
                            <IdentityAuthenticationText />
                        </Label>
                        <Box alignY="center">
                            <Switch name="identityAuthentication" />
                        </Box>

                        <Label>
                            <PhoneAuthenticationText />
                        </Label>
                        <Box alignY="center">
                            <Switch name="phoneAuthentication" />
                        </Box>

                        <Label>
                            <VipLevelsText />
                        </Label>
                        <Box alignY="center">
                            {/* TODO: brandId */}
                            <VipLevelCheckboxGroup name="vipLevelIds" />
                        </Box>
                    </FieldValues>
                </Columns.Column>
            </Columns>
            <Stack spacing="major-1" orientation="horizontal">
                <Box flex="0 auto">
                    <Trans zh="变更原因" en="Reason" />
                </Box>

                <Textarea name="reason" />
            </Stack>
        </Stack>
    )
}

const AccumulatedDepositGroup = () => {
    let { watch } = useFormContext()
    let depositedAmountMin = watch("depositedAmountMin")
    let depositedAmountMax = watch("depositedAmountMax")
    let disabledDepositedAmountMax = watch("depositedAmountUnlimited")

    return (
        <Stack orientation="horizontal" alignY="center">
            <NumberRangeInputGroup
                names={["depositedAmountMin", "depositedAmountMax"]}
                required
                min={[0, depositedAmountMin]}
                max={[depositedAmountMax]}
                disabled={[false, disabledDepositedAmountMax]}
                width="100%"
            />
            <Checkbox
                name="depositedAmountUnlimited"
                checkboxLabel={<UnlimitedText />}
                whiteSpace="nowrap"
            />
        </Stack>
    )
}

const ValueBlock = ({ children, ...props }) => (
    <Text.Block alignY="center" {...props}>
        {children}
    </Text.Block>
)

const List = applyTheme(_List, {
    styles: {
        base: {
            border: "default"
        }
    }
})

const UploadListColumn = ({ name = "", title, fileName = "" }) => {
    let { control, setValue, watch } = useFormContext()
    let list = watch(name) ?? []

    let uploadFileInputRef = useRef()
    let [formData, setFormData] = useState()
    let {
        result,
        isLoading,
        error,
        clearCache,
        deleteCache
    } = useBrandPaymentUpload(formData)

    const resetFormData = () => {
        if (uploadFileInputRef?.current) {
            uploadFileInputRef.current.value = ""
        }
        deleteCache()
        setFormData(undefined)
    }

    list = result?.players ?? list

    useEffect(() => {
        if (Array.isArray(result?.players) && !error) {
            setValue(name, result?.players)
        }
    }, [formData, result])

    return (
        <Controller
            control={control}
            name={name}
            render={({
                field: { onChange, onBlur, value, name, ref },
                fieldState: { invalid, isTouched, isDirty }
            }) => (
                <Columns.Column display="flex" flexDirection="column">
                    {error && (
                        <AlertDialog {...error} onClickClose={resetFormData} />
                    )}
                    <Stack
                        orientation="horizontal"
                        alignY="center"
                        spacing="major-1"
                    >
                        <Text.Block>{title}</Text.Block>
                        <Button
                            use="a"
                            href="/download/payments/list.xlsx"
                            download={fileName}
                            palette="primary"
                            size="small"
                        >
                            <Trans zh="格式" en="Format" />
                        </Button>
                    </Stack>
                    <List overflowY="scroll" flex="1 0 5em" marginY="minor-1">
                        {list.map((item) => (
                            <List.Item key={item}>{item}</List.Item>
                        ))}
                    </List>
                    <Set alignX="center" orientation="horizontal">
                        <input
                            ref={uploadFileInputRef}
                            type="file"
                            onChange={(event) => {
                                let [file] = event.target.files
                                if (file) {
                                    let formData = new FormData()
                                    formData.append("file", file, file.name)
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
                            palette="primary"
                            type="button"
                            onClick={(event) => {
                                uploadFileInputRef?.current.click()
                            }}
                            size="small"
                            isLoading={isLoading}
                            disabled={isLoading}
                        >
                            <Trans zh="上传" en="Upload" />
                        </Button>
                        <Button
                            type="button"
                            size="small"
                            onClick={() => {
                                setValue(name, [])
                            }}
                        >
                            <Trans zh="清除" en="Clear" />
                        </Button>
                    </Set>
                </Columns.Column>
            )}
        />
    )
}
