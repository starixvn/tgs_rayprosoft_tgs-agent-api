import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    FieldStack,
    Box,
    Button,
    Text,
    Modal,
    Set,
    useModalState,
    Columns,
    Heading,
    Level,
    Dialog
} from "bumbag"
import {
    Create as CreateMenu,
    Update as UpdateMenu,
    View as ViewMenu
} from "./menu"
import {
    useIsDisplayMenuByRoute,
    useUserBrandId,
    useUserLicenseeId,
    useUserResellerId,
    useUserRoleType
} from "../../user/context"
import {
    FormattedNumber,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import {
    AddDisclosure,
    EditDisclosure,
    ViewDisclosure
} from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { FieldValues } from "../../layout/Grid"
import Input from "../../input/Input"
import { useAddVipLevel, useUpdateVipLevel } from "../../vip-levels/client"
import { NormalLabel } from "../../form/Label"
import Checkbox from "../../checkbox"
import {
    CloseButtonFooter,
    ConfirmCancelFooter
} from "../../form/ConfirmButtonFooter"
import { formatNumberDecimal } from "../../number/format"
import { MerchantIdHookFormSelect } from "../../merchant/MerchantSelect"
import { nextRoleType } from "../../system/roles/type"

export const AddButton = ({ onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([CreateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <AddDisclosure {...dialog} />
            {dialog.visible && (
                <AddDialog onSubmitted={onSubmitted} dialog={dialog} />
            )}
        </>
    )
}

const AddDialog = ({ onSubmitted, dialog }) => {
    let userBrandId = useUserBrandId()
    let [formData, setSubmit] = useState()
    let { result, error, mutate, deleteCache, isLoading } = useAddVipLevel(
        formData
    )

    const onSubmit = (data) => {
        setSubmit(data)
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let title = useTrans({
        zh: "新增等级",
        en: "Add VIP Level"
    })

    return (
        <FormDialog
            defaultValues={{
                brandId: userBrandId
            }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            showActionButtons={false}
            footer={<ConfirmCancelFooter dialog={dialog} />}
            maxWidth="100vw"
            minWidth="70vw"
            placement="top"
        >
            <EditBody isAdd={true} />
        </FormDialog>
    )
}

export const EditButton = ({ record, onSubmitted, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([UpdateMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <EditDisclosure {...dialog} {...props} />
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

const EditDialog = ({ record, onSubmitted, dialog, ...props }) => {
    let { id } = record

    let [formData, setSubmit] = useState()
    let { result, error, mutate, deleteCache, isLoading } = useUpdateVipLevel(
        id,
        formData
    )

    let title = useTrans({
        zh: "编辑等级",
        en: "Edit VIP Level"
    })

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
            defaultValues={{ ...record }}
            onSubmit={onSubmit}
            isLoading={isLoading}
            result={result}
            error={error}
            onCloseErrorAlert={reset}
            onSubmitted={onSubmitted}
            title={title}
            dialog={dialog}
            showActionButtons={false}
            footer={<ConfirmCancelFooter dialog={dialog} />}
            maxWidth="100vw"
            minWidth="70vw"
            placement="top"
        >
            <EditBody />
        </FormDialog>
    )
}

const EditBody = ({ record, readonly = false, isAdd = false }) => {
    let userRoleType = useUserRoleType()
    let brandId = useUserBrandId()

    let editable = !readonly
    return (
        <FieldStack orientation="vertical">
            {isAdd && !brandId && (
                <MerchantIdHookFormSelect start={nextRoleType(userRoleType)} />
            )}
            <Level width="100%">
                <Set orientation="vertical" isFilled flex="1">
                    <Heading use="h5">
                        <Trans zh="升级条件" en="Upgrade conditions" />
                    </Heading>
                    <Set
                        orientation="vertical"
                        marginLeft="1rem"
                        marginRight="1rem"
                        isFilled
                    >
                        <FieldValues>
                            <NormalLabel>
                                <Trans
                                    zh="累积投注金额 （当月）"
                                    en="Accumulated bet amount (current month)"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="levelUpAccumulateBetAmount "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={
                                            record?.levelUpAccumulateBetAmount
                                        }
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans
                                    zh="累积充值金额 （当月）"
                                    en="Accumulated deposit amount (current month)"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="levelUpAccumulateDeposit "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={record?.levelUpAccumulateDeposit}
                                    />
                                </Box>
                            )}
                        </FieldValues>
                    </Set>
                </Set>
                <Set orientation="vertical" isFilled flex="1">
                    <Heading use="h5">
                        <Trans
                            zh="等级维持条件"
                            en="Level maintenance conditions"
                        />
                    </Heading>
                    <Set
                        orientation="vertical"
                        isFilled
                        marginLeft="1rem"
                        marginRight="1rem"
                    >
                        <FieldValues>
                            <NormalLabel>
                                <Trans
                                    zh="累积投注金额 （每季）"
                                    en="Accumulated bet amount (per quarter)"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="levelKeepAccumulateBetAmount "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={
                                            record?.levelKeepAccumulateBetAmount
                                        }
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans
                                    zh="累积充值金额 （每季）"
                                    en="Accumulated deposit amount (per quarter)"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="levelKeepAccumulateDeposit "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={
                                            record?.levelKeepAccumulateDeposit
                                        }
                                    />
                                </Box>
                            )}
                        </FieldValues>
                        {editable && (
                            <Checkbox
                                name="levelKeepCondition"
                                label={
                                    <Trans
                                        zh="无需等级维持条件"
                                        en="Disable maintenance conditions"
                                    />
                                }
                            />
                        )}
                        {readonly && (
                            <Checkbox
                                value={record?.levelKeepCondition}
                                disabled
                                label={
                                    <Trans
                                        zh="无需等级维持条件"
                                        en="Disable maintenance conditions"
                                    />
                                }
                            />
                        )}
                    </Set>
                </Set>
            </Level>

            <Box
                border="default"
                height="1px"
                width="100%"
                borderBottom="none"
            ></Box>

            <Level width="100%">
                <Set orientation="vertical" isFilled flex="1">
                    <Heading use="h5">
                        <Trans zh="VIP福利" en="VIP benefits" />
                    </Heading>
                    <Set
                        orientation="vertical"
                        marginLeft="1rem"
                        marginRight="1rem"
                        isFilled
                    >
                        <FieldValues>
                            <NormalLabel>
                                <Trans zh="生日福利" en="Birthday benefit" />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input type="number" name="birthBenefit" />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={record?.birthBenefit}
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans zh="晋升福利" en="Promotion benefit" />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="promotionBenefit "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={record?.promotionBenefit}
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans zh="季度福利" en="Quarterly benefit" />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="quarterlyBenefit "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumber
                                        value={record?.quarterlyBenefit}
                                    />
                                </Box>
                            )}
                        </FieldValues>
                    </Set>
                </Set>
                <Set orientation="vertical" isFilled flex="1">
                    <Heading use="h5">
                        <Trans zh="VIP反水比例" en="VIP rebate percentage " />
                    </Heading>
                    <Set
                        orientation="vertical"
                        marginLeft="1rem"
                        marginRight="1rem"
                        isFilled
                    >
                        <FieldValues>
                            <NormalLabel>
                                <Trans
                                    zh="真人反水比例（%）"
                                    en="Live Dealer rebate （%）"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="liveDealerRebate "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumberDecimal
                                        value={record?.liveDealerRebate}
                                        postfix="%"
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans
                                    zh="电子反水比例（%）"
                                    en="Slot Game rebate （%）"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="slotGameRebate "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumberDecimal
                                        value={record?.slotGameRebate}
                                        postfix="%"
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans
                                    zh="棋牌反水比例（%）"
                                    en="Card Game rebate （%）"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="cardGameRebate"
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumberDecimal
                                        value={record?.cardGameRebate}
                                        postfix="%"
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans
                                    zh="体育反水比例（%）"
                                    en="Sportsbook rebate （%）"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input type="number" name="sportsRebate" />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumberDecimal
                                        value={record?.sportsRebate}
                                        postfix="%"
                                    />
                                </Box>
                            )}

                            <NormalLabel>
                                <Trans
                                    zh="虚拟体育反水比例（%）"
                                    en="VirtualSports rebate （%）"
                                />
                            </NormalLabel>
                            {editable && (
                                <Box>
                                    <Input
                                        type="number"
                                        name="virtualSportsRebate "
                                    />
                                </Box>
                            )}
                            {readonly && (
                                <Box alignY="center">
                                    <FormattedNumberDecimal
                                        value={record?.virtualSportsRebate}
                                        postfix="%"
                                    />
                                </Box>
                            )}
                        </FieldValues>
                    </Set>
                </Set>
            </Level>
        </FieldStack>
    )
}

export const ViewButton = ({ record, ...props }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([ViewMenu])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <ViewDisclosure {...dialog} {...props} />
            {dialog.visible && <ViewDialog record={record} dialog={dialog} />}
        </>
    )
}

const ViewDialog = ({ record, dialog }) => {
    let title = useTrans({
        zh: "查看等级",
        en: "View VIP Level"
    })
    return (
        <FormDialog
            defaultValues={record}
            onCloseErrorAlert={dialog.hide}
            title={title}
            dialog={dialog}
            showActionButtons={false}
            footer={<CloseButtonFooter dialog={dialog} />}
            maxWidth="100vw"
            minWidth="70vw"
            placement="top"
        >
            <EditBody record={record} readonly={true} />
        </FormDialog>
    )
}
