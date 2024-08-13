# UnRando

A [Randomizer 4](https://github.com/homothetyhk/RandomizerMod) connection which randomizes items by check count only, instead of location.

This connection greatly reduces the rng of randomizer races, allowing "check velocity", or checks-per-minute, to be the only meaningful factor in determining the victor of a race. Two players playing the same seed will always find item X first, item Y second, item Z third, etc., regardless of _where_ they go to find these items.

## Settings

UnRando allows explicitly setting the approximate progression depth of major movement items.

Progression can be set to 'random' (unmanaged), 'early', 'average', 'late', or 'very late'. The latter 4 settings will force progression to a specific check number relative to the total number of checks, guaranteeing early or late discovery as requested.

## Recommendations

UnRando is not compatible with split groups by its very nature, so you must turn them all off.

Dupes are allowed but generally break the premise of UnRando, so it is recommended to turn dupes off. They will be irrelevant most of the time anyways.

## Integrations

UnRando is compatible with ItemSync and integrates with RandoSettingsManager.

UnRando is not compatible with MultiWorld (yet). It may not work well with with custom shops, like Lemm shop and Grass shop.

## Details

Shops are hard-coded to only contain a fixed number of checks for balance. Iselda always has 1 item, all other shops (Sly+key and Salubra+charms count as separate shops) have 2 items.

Sometimes there are more items to obtain than there are locations to find them at, in which case UnRando will stack additional items into the same check(s) to compensate. This means you may sometimes see multilple items for sale in a single slot at a shop.

Lifeblood and Soul refills, once obtained, become permanently located wherever they were obtained. This means that if you see lifeblood at a shop, stag, etc., you can choose not to purchase it, and instead do another check first, knowing that it *will* be lifeblood and the lifeblood will be forever renewable at that location. Use this info wisely.

On the negative side, this also means that if you see a Dreamer at a shop, stag, etc., you cannot judiciously avoid purchasing it to delay infection. Your next check *will* be a Dreamer, no matter what.
