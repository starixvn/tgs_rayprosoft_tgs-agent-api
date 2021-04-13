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
    useClassName
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
            onSubmitted && onSubmitted()
        }
    }, [result])

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
                                        <Input name="parentAgentUsername" />
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
                                <AgentTypeSelect
                                    name="type"
                                    disabled={!agentHead}
                                />
                            </AgentTypeBox>

                            <BounsBox>
                                <InputField
                                    type="number"
                                    name="dividendPercentage"
                                    step="0.1"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </BounsBox>

                            <RebateBox
                                label={
                                    <Trans
                                        zh="真人返佣"
                                        en="Live Dealer Rebate"
                                    />
                                }
                            >
                                <InputField
                                    type="number"
                                    name="liveDealerRebatePercentage"
                                    step="0.01"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </RebateBox>

                            <RebateBox
                                label={<Trans zh="电子返佣" en="Slot Rebate" />}
                            >
                                <InputField
                                    type="number"
                                    name="slotRebatePercentage"
                                    step="0.01"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </RebateBox>

                            <RebateBox
                                label={
                                    <Trans
                                        zh="棋牌返佣"
                                        en="Card Game Rebate"
                                    />
                                }
                            >
                                <InputField
                                    type="number"
                                    name="cardGameRebatePercentage"
                                    step="0.01"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </RebateBox>

                            <RebateBox
                                label={
                                    <Trans zh="彩票返佣" en="Lottery Rebate" />
                                }
                            >
                                <InputField
                                    type="number"
                                    name="lotteryRebatePercentage"
                                    step="0.01"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </RebateBox>

                            <RebateBox
                                label={
                                    <Trans zh="体育返佣" en="Sports Rebate" />
                                }
                            >
                                <InputField
                                    type="number"
                                    name="sportsRebatePercentage"
                                    step="0.01"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </RebateBox>

                            <RebateBox
                                label={
                                    <Trans
                                        zh="虚拟体育返佣"
                                        en="Virtual Sports Rebate"
                                    />
                                }
                            >
                                <InputField
                                    type="number"
                                    name="virtualSportsRebatePercentage"
                                    step="0.01"
                                    min="0"
                                    addonAfter={<Button isStatic>%</Button>}
                                />
                            </RebateBox>
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

const ViewPanel = ({ record, ...props }) => {
    return (
        <TabPanel tabId={Key} {...props}>
            <FieldValues>
                <UsernameBox value={record?.username} />

                <RealNameBox value={record?.realName} />

                <ParentAgentUsername value={record?.parentAgentUsername} />

                <RegistrationBox value={record?.registeredOn} />

                <BalanceBox value={record?.balance} />

                <AgentStatusBox value={record?.status} />

                <LevelTypeBox value={record?.levelType} />

                <AgentTypeBox value={record?.type} />

                <BounsBox value={record?.bonusPercentage} />

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
                    value={record?.liveDealerRebatePercentage}
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
            {/* <Trans zh="返佣比例" en="Rebate Percentage" /> */}
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
