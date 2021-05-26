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
import { AddDisclosure, EditDisclosure } from "../../dialog/Disclosure"
import { ConfirmButtonFooter } from "../../form/ConfirmButtonFooter"
import { FieldValues } from "../../layout/Grid"
import { NormalLabel as Label } from "../../form/Label"
import BrandPaymentStatus, {
    BrandPaymentStatusButton
} from "./BrandPaymentStatus"
import {
    useUpdatePayment,
    useBrandPaymentUpload,
    useAddBrandPayment,
    useBrandPaymentMethodChannelList
} from "./client"
import AlertDialog from "../../dialog/Alert"
import {
    AllPlayerText,
    AccumulatedDepositText,
    UnlimitedText,
    IdentityAuthenticationText,
    PhoneAuthenticationText,
    VipLevelsText,
    PlayerListConditionRadioGroup,
    PlayerListCondition
} from "./condition"
import { VipLevelCheckboxGroup } from "../../vip-levels/CheckboxGroup"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import { NumberRangeInputGroup } from "../../number/RangeInput"
import FormDialog from "../../form/FormDialog"
import Textarea from "../../textarea"
import Input from "../../input/Input"
import { Checkbox, CheckboxField } from "../../checkbox/Checkbox"
import { Switch } from "../../switch/Switch"
import clone from "clone-deep"
import { usePaymentList } from "../../payment/client"
import { PaymentProviderLabel } from "../../payment/PaymentProvider"
import {
    PaymentMethodLabel,
    PaymentMethodSelect
} from "../../payment/method/PaymentMethod"
import {
    PaymentChannelLabel,
    PaymentChannelSelect
} from "../../payment/channel/PaymentChannel"
import { parseJSON } from "../../text/json"

export const EditButton = ({ brandId, record, onSubmitted, ...props }) => {
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
                    brandId={brandId}
                    record={record}
                    onSubmitted={onSubmitted}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const EditDialog = ({
    brandId,
    record: { extendedData, ...record } = {},
    onSubmitted,
    dialog,
    ...props
}) => {
    let title = useTrans({
        zh: "编辑",
        en: "Edit"
    })

    let {
        isLoading: isLoadingPayment,
        manualPayment,
        bankTransferPaymentMethod,
        bankTransferPaymentMethodChannels,
        error: loadingPaymentError
    } = useBankTransferPayment()

    let [submitData, setSubmit] = useState()

    let { result, isLoading, error, mutate, deleteCache } = useUpdatePayment(
        record.id,
        submitData
    )

    if (isLoadingPayment) {
        return <LoadingBackdrop {...dialog} />
    }

    if (loadingPaymentError) {
        return (
            <AlertDialog {...loadingPaymentError} onClickClose={dialog.hide} />
        )
    }

    extendedData = parseJSON(extendedData)
    return (
        <FormDialog
            defaultValues={clone({
                ...record,
                extendedData,
                reason: ""
            })}
            onSubmit={(data) => {
                setSubmit({
                    brandId,
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
            <EditBody
                brandId={brandId}
                record={record}
                manualPayment={manualPayment}
                bankTransferPaymentMethod={bankTransferPaymentMethod}
                bankTransferPaymentMethodChannels={
                    bankTransferPaymentMethodChannels
                }
            />
        </FormDialog>
    )
}

export const AddBrandPaymentButton = ({ brandId, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
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

const useBankTransferPayment = () => {
    let {
        isLoading: isLoadingPaymentList,
        data: paymentListData,
        error: paymentListError,
        mutate: mutatePaymentList,
        deleteCache: deletePaymentListCache
    } = usePaymentList()

    let {
        isLoading: isLoadingBrandPaymentMethodChannelList,
        data: paymentMethodChannelList,
        error: paymentMethodChannelListError,
        mutate: mutatePaymentMethodChannelList,
        deleteCache: deletePaymentMethodChannelListCache
    } = useBrandPaymentMethodChannelList()

    let manualPayment = paymentListData?.find(({ code }) => code === "Manual")

    let bankTransferPaymentMethod = paymentMethodChannelList?.methods?.find(
        ({ code }) => code === "BankTransfer"
    )

    let bankTransferPaymentMethodChannels =
        paymentMethodChannelList?.allData?.filter((item) => {
            return item.methodId === bankTransferPaymentMethod.id
        })

    /*
    let manualBankTransferPaymentMethodChannels =
        paymentMethodChannelList?.allData?.filter((item) => {
            return (
                item.methodId === bankTransferPaymentMethod.id &&
                item.paymentId === manualPayment.id
            )
        })
    */

    return {
        isLoading:
            isLoadingPaymentList || isLoadingBrandPaymentMethodChannelList,
        manualPayment,
        bankTransferPaymentMethod,
        bankTransferPaymentMethodChannels,
        error: paymentListError || paymentMethodChannelListError,
        mutatePaymentList,
        mutatePaymentMethodChannelList,
        deleteCache: () => {
            deletePaymentListCache()
            deletePaymentMethodChannelListCache()
        }
    }
}

const AddDialog = ({ brandId, onSubmitted, dialog, ...props }) => {
    let {
        isLoading: isLoadingPayment,
        manualPayment,
        bankTransferPaymentMethod,
        bankTransferPaymentMethodChannels,
        error: loadingPaymentError
    } = useBankTransferPayment()

    let title = useTrans({
        zh: "新增",
        en: "Add"
    })

    let [formData, setSubmit] = useState()
    let { result, isLoading, error, mutate, deleteCache } =
        useAddBrandPayment(formData)

    if (isLoadingPayment) {
        return <LoadingBackdrop {...dialog} />
    }

    if (loadingPaymentError) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    return (
        <FormDialog
            defaultValues={{
                paymentStatus: BrandPaymentStatus.Enabled,
                playerListCondition: PlayerListCondition.MatchOne,
                allPlayers: true,
                depositedAmountMin: "0",
                extendedData: {}
            }}
            onSubmit={onSubmit}
            result={result}
            isLoading={isLoading}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
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
            <EditBody
                brandId={brandId}
                isAdd={true}
                manualPayment={manualPayment}
                bankTransferPaymentMethod={bankTransferPaymentMethod}
                bankTransferPaymentMethodChannels={
                    bankTransferPaymentMethodChannels
                }
            />
        </FormDialog>
    )
}

const EditBody = ({
    brandId,
    isAdd = false,
    record = {},
    manualPayment = {},
    bankTransferPaymentMethod,
    bankTransferPaymentMethodChannels = [],
    dialog,
    ...props
}) => {
    let { watch, setValue, getValues } = useFormContext()
    let transferLimitTo = watch("transferLimitTo")
    let transferLimitFrom = watch("transferLimitFrom")

    const PaymentChannelIdFieldName = "paymentChannelId"
    let paymentChannelId = watch("paymentChannelId")

    let paymentMethodId = watch("paymentMethodId")
    const PaymentMethodChannelIdFieldName = "paymentMethodChannelId"
    let paymentMethodChannelId = watch(PaymentMethodChannelIdFieldName)
    let isEdit = !isAdd
    let paymentId = isAdd ? manualPayment?.id : record?.paymentId
    let isManualPayment = paymentId == manualPayment?.id
    let isBankTransferMethod = paymentMethodId === bankTransferPaymentMethod?.id
    useEffect(() => {
        if (isBankTransferMethod) {
            let paymentMethodChannel = bankTransferPaymentMethodChannels.find(
                (item) => {
                    return (
                        item.paymentId === paymentId &&
                        item.methodId === paymentMethodId &&
                        item.channelId === paymentChannelId
                    )
                }
            )
            setValue(
                PaymentMethodChannelIdFieldName,
                paymentMethodChannel?.id ?? ""
            )
        }
    }, [paymentChannelId, paymentMethodId])
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
                                <Trans zh="支付厂商" en="Payment Name" />
                            </Label>
                            <Box alignY="center">
                                <Input
                                    type="hidden"
                                    name="paymentId"
                                    value={paymentId}
                                />
                                <PaymentProviderLabel value={paymentId} />
                            </Box>

                            <Label>
                                <Trans zh="支付方式" en="Payment Medthod" />
                            </Label>
                            <Box alignY="center">
                                {isAdd && (
                                    <PaymentMethodSelect
                                        name="paymentMethodId"
                                        placeholder={false}
                                        range={[bankTransferPaymentMethod.id]}
                                        value={bankTransferPaymentMethod?.id}
                                    />
                                )}
                                {isEdit && isBankTransferMethod && (
                                    <PaymentMethodSelect
                                        name="paymentMethodId"
                                        range={[bankTransferPaymentMethod.id]}
                                        required
                                    />
                                )}
                                {isEdit && !isBankTransferMethod && (
                                    <PaymentMethodLabel
                                        range={[bankTransferPaymentMethod.id]}
                                        value={record?.paymentMethodId}
                                    />
                                )}
                                <Input
                                    type="hidden"
                                    name={PaymentMethodChannelIdFieldName}
                                    value={paymentMethodChannelId}
                                />
                            </Box>

                            <Label>
                                <Trans zh="支付通道" en="Payment Channel" />
                            </Label>
                            <Box alignY="center">
                                {isAdd && (
                                    <PaymentChannelSelect
                                        name={PaymentChannelIdFieldName}
                                        required
                                    />
                                )}
                                {isEdit && isBankTransferMethod && (
                                    <PaymentChannelSelect
                                        name={PaymentChannelIdFieldName}
                                        required
                                    />
                                )}
                                {isEdit && !isBankTransferMethod && (
                                    <PaymentChannelLabel
                                        value={record?.paymentChannelId}
                                    />
                                )}
                            </Box>

                            <Label required>
                                <Trans zh="通道名称" en="Channel Name" />
                            </Label>
                            <Input name="channelName" size="small" required />

                            {isManualPayment && (
                                <>
                                    <Label required>
                                        <Trans
                                            zh="帐号名称"
                                            en="Account Name"
                                        />
                                    </Label>
                                    <Input name="extendedData.name" required />

                                    <Label required>
                                        <Trans
                                            zh="银行卡号"
                                            en="Bank Account"
                                        />
                                    </Label>
                                    <Input
                                        name="extendedData.bankAccount"
                                        required
                                    />
                                </>
                            )}

                            <Label>
                                <Trans zh="状态" en="Status" />
                            </Label>
                            <Box>
                                <BrandPaymentStatusButton name="paymentStatus" />
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
                            <PlayerListConditionRadioGroup
                                name="playerListCondition"
                                required
                            />
                        </Box>

                        <Label name="allPlayers">
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
                            <VipLevelCheckboxGroup
                                name="vipLevelIds"
                                brandId={brandId}
                            />
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
            <Input type="hidden" name="brandId" value={brandId} />
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
            <CheckboxField
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
    let { result, isLoading, error, clearCache, deleteCache } =
        useBrandPaymentUpload(formData)

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
            shouldUnregister={true}
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
