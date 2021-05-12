import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import {
    Spinner,
    Stack,
    Flex,
    Box,
    Card,
    Set,
    Button,
    Text,
    Dialog,
    Modal,
    Table,
    useClassName,
    Alert
} from "bumbag"
import { FieldValues } from "../../layout/Grid"
import {
    Trans,
    FormattedDateTime,
    FormattedNumber,
    useTrans,
    FormattedNumberDecimal
} from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import { Input, InputField } from "../../input/Input"
import { useAgent, useUpdateAgent } from "./client"
import { AgentStatusLabel, AgentStatusSelect } from "../AgentStatus"
import { AgentLevelLabel, AgentLevelSelect, isAgentHead } from "../AgentLevel"
import { AgentTypeLabel, AgentTypeSelect } from "../AgentType"
import clone from "clone-deep"
import Select from "../../select/Select"
import { PercentageNumberInputField } from "../../input/NumberInput"
import { AgentCommissionPercentageInputField } from "../AgentCommissionPercentage"
import { AgentLiveDealerRebatePercentageInputField } from "../AgentLiveDealerRebatePercentage"
import { AgentEgameRebatePercentageInputField } from "../AgentEgameRebatePercentage"
import { AgentCardGameRebatePercentageInputField } from "../AgentCardGameRebatePercentage"
import { AgentLotteryRebatePercentageInputField } from "../AgentLotteryRebatePercentage"
import { AgentSportsRebatePercentageInputField } from "../AgentSportsRebatePercentage"
import { AgentVirtualSportsRebatePercentageInputField } from "../AgentVirtualSportsRebatePercentage"

const Key = "AgentInfo"

export const AgentInfoKey = Key

export const AgentInfoTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="代理信息" en="Agent Info" />
        </_Tab>
    )
}

export const AgentInfoPanel = ({ readonly = false, ...props }) =>
    readonly ? <ViewPanel {...props} /> : <EditPanel {...props} />

const EditPanel = ({ record, onSubmitted, onClickClose, ...props }) => {
    let formMethods = useForm({
        defaultValues: clone(record)
    })

    let agentId = record?.id
    let [formData, setSubmit] = useState()
    let { isLoading, result, error, mutate, deleteCache } = useUpdateAgent(
        agentId,
        formData
    )

    useEffect(() => {
        if (result && !error) {
            reset()
            onSubmitted && onSubmitted()
        }
    }, [result])

    let agentUsernamePlaceholder = useTrans({
        zh: "代理用户名",
        en: "Agent Username"
    })

    let currentParentUsername = record.parentUsername
    let parentUsername = formMethods.watch("parentUsername")
    let changeParent =
        currentParentUsername &&
        parentUsername &&
        currentParentUsername != parentUsername

    const onSubmit = (data) => {
        setSubmit({
            ...record,
            updateType: "info",
            ...data
        })
    }

    const reset = () => {
        deleteCache()
        mutate(undefined, false)
        setSubmit(undefined)
    }

    let agentHead = isAgentHead(record?.levelType)
    return (
        <TabPanel tabId={Key} {...props}>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                    <Stack>
                        <FieldValues>
                            <UsernameBox value={record?.username} />

                            <RealNameBox>
                                <Input name="realName" />
                            </RealNameBox>

                            {!agentHead && (
                                <>
                                    <ParentAgentUsername>
                                        <Set orientation="vertical" isFilled>
                                            <Input
                                                name="parentUsername"
                                                placeholder={
                                                    agentUsernamePlaceholder
                                                }
                                            />
                                            {changeParent && (
                                                <ChangeParentWarning />
                                            )}
                                        </Set>
                                    </ParentAgentUsername>
                                </>
                            )}

                            <RegistrationBox value={record?.registeredOn} />

                            <BalanceBox value={record?.balance} />

                            <AgentStatusBox>
                                <AgentStatusSelect
                                    name="status"
                                    placeholder={false}
                                    required
                                />
                            </AgentStatusBox>

                            <AgentTypeBox>
                                <Box alignY="center">
                                    {!agentHead && (
                                        <>
                                            <AgentTypeSelect
                                                name="type"
                                                value={record?.type}
                                                display="none"
                                            />
                                            <AgentTypeSelect
                                                value={record?.type}
                                                disabled
                                            />
                                        </>
                                    )}
                                    {agentHead && (
                                        <AgentTypeSelect name="type" />
                                    )}
                                </Box>
                            </AgentTypeBox>

                            {!changeParent && (
                                <BounsBox>
                                    <AgentCommissionPercentageInputField />
                                </BounsBox>
                            )}

                            {!changeParent && (
                                <RebateBox
                                    label={
                                        <Trans
                                            zh="真人返佣"
                                            en="Live Dealer Rebate"
                                        />
                                    }
                                >
                                    <AgentLiveDealerRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!changeParent && (
                                <RebateBox
                                    label={
                                        <Trans
                                            zh="电子返佣"
                                            en="EGame Rebate"
                                        />
                                    }
                                >
                                    <AgentEgameRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!changeParent && (
                                <RebateBox
                                    label={
                                        <Trans
                                            zh="棋牌返佣"
                                            en="Card Game Rebate"
                                        />
                                    }
                                >
                                    <AgentCardGameRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!changeParent && (
                                <RebateBox
                                    label={
                                        <Trans
                                            zh="彩票返佣"
                                            en="Lottery Rebate"
                                        />
                                    }
                                >
                                    <AgentLotteryRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!changeParent && (
                                <RebateBox
                                    label={
                                        <Trans
                                            zh="体育返佣"
                                            en="Sports Rebate"
                                        />
                                    }
                                >
                                    <AgentSportsRebatePercentageInputField />
                                </RebateBox>
                            )}

                            {!changeParent && (
                                <RebateBox
                                    label={
                                        <Trans
                                            zh="虚拟体育返佣"
                                            en="Virtual Sports Rebate"
                                        />
                                    }
                                >
                                    <AgentVirtualSportsRebatePercentageInputField />
                                </RebateBox>
                            )}
                        </FieldValues>
                        <Box alignX="right">
                            <SubmitCancel
                                isLoading={isLoading}
                                onClickCancel={onClickClose}
                            />
                        </Box>
                    </Stack>
                </form>
            </FormProvider>
            {error && <AlertDialog {...error} onClickClose={reset} />}
        </TabPanel>
    )
}

const ChangeParentWarning = () => {
    let warning = useTrans({
        zh: "更改上級需重新配置",
        en: "Change the upper level agent required to re-config."
    })
    return <Alert variant="tint" title={warning} type="warning" />
}

const ViewPanel = ({ record, ...props }) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <FieldValues>
                <UsernameBox value={record?.username} />

                <RealNameBox value={record?.realName} />

                <ParentAgentUsername value={record?.parentUsername} />

                <RegistrationBox value={record?.registeredOn} />

                <BalanceBox value={record?.balance} />

                <AgentStatusBox value={record?.status} />

                <LevelTypeBox value={record?.levelType} />

                <AgentTypeBox value={record?.type} />

                <BounsBox value={record?.commissionPercentage} />

                <RebateBox
                    label={<Trans zh="真人返佣" en="Live Dealer Rebate" />}
                    value={record?.liveDealerRebatePercentage}
                />

                <RebateBox
                    label={<Trans zh="电子返佣" en="Slot Rebate" />}
                    value={record?.slotRebatePercentage}
                />

                <RebateBox
                    label={<Trans zh="棋牌返佣" en="Card Game Rebate" />}
                    value={record?.cardGameRebatePercentage}
                />

                <RebateBox
                    label={<Trans zh="彩票返佣" en="Lottery Rebate" />}
                    value={record?.lotteryRebatePercentage}
                />

                <RebateBox
                    label={<Trans zh="体育返佣" en="Sports Rebate" />}
                    value={record?.sportsRebatePercentage}
                />

                <RebateBox
                    label={
                        <Trans zh="虚拟体育返佣" en="Virtual Sports Rebate" />
                    }
                    value={record?.virtualSportsRebatePercentage}
                />
            </FieldValues>
        </TabPanel>
    )
}

const UsernameBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="代理用户名" en="Agent Username" />
            </Box>
            <Text>{value}</Text>
        </>
    )
}

const RealNameBox = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="真实姓名" en="Real name" />
            </Box>
            {children ? children : <Text>{value}</Text>}
        </>
    )
}

const ParentAgentUsername = ({ value = "", children }) => {
    return (
        <>
            <Box alignY="center">
                <Trans zh="上级代理" en="Upper level agent" />
            </Box>

            {children ? children : <Text>{value}</Text>}
        </>
    )
}

const RegistrationBox = ({ value }) => {
    return (
        <>
            <Trans zh="注册时间" en="Registration" />
            <Text>
                <FormattedDateTime value={value} />
            </Text>
        </>
    )
}

const BalanceBox = ({ value = "" }) => {
    return (
        <>
            <Trans zh="余额" en="Balance" />
            <Text>
                <FormattedNumberDecimal value={value} />
            </Text>
        </>
    )
}

const AgentStatusBox = ({ value = "", children }) => {
    return (
        <>
            <Trans zh="状态" en="Status" />

            {children ? (
                children
            ) : (
                <Text>
                    <AgentStatusLabel value={value} />
                </Text>
            )}
        </>
    )
}

const LevelTypeBox = ({ value = "", children }) => {
    return (
        <>
            <Trans zh="代理种类" en="Level Type" />

            {children ? (
                children
            ) : (
                <Text>
                    <AgentLevelLabel value={value} />
                </Text>
            )}
        </>
    )
}

const AgentTypeBox = ({ value, children }) => {
    return (
        <>
            <Trans zh="代理类型" en="Agent Type" />
            {children ? (
                children
            ) : (
                <Text>
                    <AgentTypeLabel value={value} />
                </Text>
            )}
        </>
    )
}

const BounsBox = ({ value, children }) => {
    return (
        <>
            <Trans zh="分红比例" en="Bonus Percentage" />
            {children ? (
                children
            ) : (
                <Text>
                    <FormattedNumberDecimal value={value} postfix="%" />
                </Text>
            )}
        </>
    )
}

const RebateBox = ({ label, value, children }) => {
    return (
        <>
            {label}
            {children ? (
                children
            ) : (
                <Text>
                    <FormattedNumberDecimal value={value} postfix="%" />
                </Text>
            )}
        </>
    )
}
