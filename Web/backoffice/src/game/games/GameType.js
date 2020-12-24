import React, { useState, useEffect, useMemo } from "react"
import {
  css,
  Stack,
  Heading,
  Box,
  Button,
  Select,
  SelectMenu,
  DropdownMenu
} from "bumbag"
import { useTrans, Trans } from "../../i18n"

const Sportsbook = "Sportsbook"
const SportsbookTrans = {
  zh: "Sportsbook",
  en: "Sportsbook"
}

const VirtualSports = "VirtualSports"
const VirtualSportsTrans = {
  zh: "VirtualSports",
  en: "VirtualSports"
}

const ESports = "ESports"
const ESportsTrans = {
  zh: "ESports",
  en: "ESports"
}

const LiveDealer = "LiveDealer"
const LiveDealerTrans = {
  zh: "LiveDealer",
  en: "LiveDealer"
}

const LiveDealerLobby = "LiveDealerLobby"
const LiveDealerLobbyTrans = {
  zh: "LiveDealerLobby",
  en: "LiveDealerLobby"
}

const CardGame = "CardGame"
const CardGameTrans = {
  zh: "CardGame",
  en: "CardGame"
}

const CardGameLobby = "CardGameLobby"
const CardGameLobbyTrans = {
  zh: "CardGameLobby",
  en: "CardGameLobby"
}

const Fishing = "Fishing"
const FishingTrans = {
  zh: "Fishing",
  en: "Fishing"
}

const FishingLobby = "FishingLobby"
const FishingLobbyTrans = {
  zh: "FishingLobby",
  en: "FishingLobby"
}

const Arcade = "Arcade"
const ArcadeTrans = {
  zh: "Arcade",
  en: "Arcade"
}

const ArcadeLobby = "ArcadeLobby"
const ArcadeLobbyTrans = {
  zh: "ArcadeLobby",
  en: "ArcadeLobby"
}

const Slot = "Slot"
const SlotTrans = {
  zh: "Slot",
  en: "Slot"
}

const Lottery = "Lottery"
const LotteryTrans = {
  zh: "Lottery",
  en: "Lottery"
}

const TableGame = "TableGame"
const TableGameTrans = {
  zh: "TableGame",
  en: "TableGame"
}

export const GameTypeLabel = ({ value }) => {
  switch (value) {
    case Sportsbook:
      return <Trans {...SportsbookTrans} />

    case VirtualSports:
      return <Trans {...VirtualSportsTrans} />

    case ESports:
      return <Trans {...ESportsTrans} />

    case LiveDealer:
      return <Trans {...LiveDealerTrans} />

    case LiveDealerLobby:
      return <Trans {...LiveDealerLobbyTrans} />

    case CardGame:
      return <Trans {...CardGameTrans} />

    case CardGameLobby:
      return <Trans {...CardGameLobbyTrans} />

    case Fishing:
      return <Trans {...FishingTrans} />

    case FishingLobby:
      return <Trans {...FishingLobbyTrans} />

    case Arcade:
      return <Trans {...ArcadeTrans} />

    case ArcadeLobby:
      return <Trans {...ArcadeLobbyTrans} />

    case Slot:
      return <Trans {...SlotTrans} />

    case Lottery:
      return <Trans {...LotteryTrans} />

    case TableGame:
      return <Trans {...TableGameTrans} />

    default:
      return value ?? null
  }
}
